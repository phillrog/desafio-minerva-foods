using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Infrastructure.Consumers;
using DesafioMinervaFoods.Infrastructure.Messaging;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class MessagingConfig
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitSettings = configuration.GetSection("RabbitMq");

            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderCreatedConsumer>();
                x.AddConsumer<RegisterOrderConsumer>();
                x.AddConsumer<OrderNotificationConsumer>();
                x.AddConsumer<ApproveOrderConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitSettings["Host"], h =>
                    {
                        h.Username(rabbitSettings["Username"]);
                        h.Password(rabbitSettings["Password"]);
                    });

                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    cfg.UseInMemoryOutbox();

                    // DICA: Você pode criar um método privado para configurar os endpoints e limpar este bloco
                    ConfigureEndpoints(cfg, context);
                });
            });

            services.AddScoped<IEventBus, MassTransitEventBus>();
            return services;
        }

        private static void ConfigureEndpoints(IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
        {
            // Configuração de Resiliência com Polly (Retry) embutido no MassTransit
            cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            // Apenas para não ter que construir o outbox pattern
            cfg.UseInMemoryOutbox();

            cfg.ReceiveEndpoint("order-created-queue", e =>
            {
                // Define o nome da Exchange de Dead Letter
                // O RabbitMQ usará essa exchange para rotear mensagens que falharem
                e.BindDeadLetterQueue("order-created-queue-dead-letter-exchange", "order-created-queue-poison-messages", cb =>
                {
                    // Aqui você pode definir regras extras, como expiração
                    cb.Durable = true;
                });

                // 1. POLÍTICA DE RETRY (Resiliência)
                // Se o consumer lançar exceção, o MassTransit fará o NACK e tentará novamente 3 vezes.
                e.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                // 2. CIRCUIT BREAKER (Opcional, mas Senior)
                // Se a fila falhar consecutivamente por muito tempo, o Circuit Breaker "abre"
                // para não sobrecarregar o banco de dados.
                e.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15; // 15% de falhas
                    cb.ActiveThreshold = 10; // Mínimo de 10 mensagens
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });
                e.ConfigureConsumer<OrderCreatedConsumer>(context);
            });

            cfg.ReceiveEndpoint("register-order-queue", e =>
            {
                // 1. CONFIGURAÇÃO DE DEAD LETTER (O "Cemitério" de mensagens)
                // Se o registro do pedido falhar (ex: erro de banco ou mapeamento), a mensagem não se perde.
                e.BindDeadLetterQueue("register-order-dead-letter-exchange", "register-order-poison-messages", cb =>
                {
                    cb.Durable = true;
                });

                // 2. POLÍTICA DE RETRY
                // Tenta processar o registro 3 vezes com intervalo de 5 segundos.
                // Útil se o SQL Server der um "timeout" momentâneo.
                e.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                // 3. CIRCUIT BREAKER
                // Se o banco de dados cair, o circuito abre e para de tentar registrar novos pedidos
                // evitando logs infinitos de erro e consumo de CPU desnecessário.
                e.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                // Registra o Consumer que criamos com o Mapper
                e.ConfigureConsumer<RegisterOrderConsumer>(context);
            });

            cfg.ReceiveEndpoint("order-notification-queue", e =>
            {
                // 1. DEAD LETTER QUEUE (Para notificações que falharem)
                e.BindDeadLetterQueue("order-notification-dead-letter-exchange", "order-notification-poison-messages", cb =>
                {
                    cb.Durable = true;
                });

                // 2. RETRY (Essencial para SignalR, que pode ter timeouts de conexão)
                e.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(2)); // Tenta 3 vezes rapidinho
                });

                // 3. CIRCUIT BREAKER
                e.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                // Configura o Consumer de Notificação que vai "falar" com o SignalR
                e.ConfigureConsumer<OrderNotificationConsumer>(context);
            });

            cfg.ReceiveEndpoint("approve-order-queue", e =>
            {
                // 1. CONFIGURAÇÃO DE DEAD LETTER
                // Se a aprovação falhar após todas as tentativas, a mensagem vai para cá.
                e.BindDeadLetterQueue("approve-order-dead-letter-exchange", "approve-order-poison-messages", cb =>
                {
                    cb.Durable = true;
                });

                // 2. POLÍTICA DE RETRY
                // Em aprovações, o Retry é vital caso haja um "concurrency conflict" (dois processos tentando atualizar o pedido ao mesmo tempo).
                e.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                // 3. CIRCUIT BREAKER
                // Protege o sistema se o serviço de persistência estiver instável.
                e.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                // Registra o Consumer de Aprovação que acabamos de criar
                e.ConfigureConsumer<ApproveOrderConsumer>(context);
            });
        }
    }
}
