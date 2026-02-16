
# DesafioMinervaFoods

Sistema de Gestão de Pedidos para Minerva Foods desenvolvido com React Native + Expo.

## 🚀 Funcionalidades

- **Autenticação**: Login com usuário e senha conectado à API local
- **Listagem de Pedidos**: Visualização de todos os pedidos com status coloridos
- **Detalhes do Pedido**: Informações completas de cada pedido
- **Criação de Pedidos**: Formulário para criar novos pedidos
- **Aprovação de Pedidos**: Aprovar pedidos que requerem aprovação manual
- **Notificações em Tempo Real**: SignalR para atualizações instantâneas
- **Status Numéricos**: Sistema de status baseado em enum (0-9)

## 📊 Status dos Pedidos

- **0**: Processando (Laranja)
- **1**: Criado (Azul)
- **2**: Pago (Verde)
- **3**: Cancelado (Vermelho)
- **9**: Erro (Cinza)

## 🔌 API Endpoints

A aplicação se conecta aos seguintes endpoints em `http://localhost:5001`:

- `POST /api/auth/login` - Autenticação
- `GET /api/Orders` - Lista de pedidos
- `GET /api/Orders/{id}` - Detalhes do pedido
- `POST /api/Orders` - Criar pedido
- `PUT /api/Orders/{id}/approve` - Aprovar pedido
- `DELETE /api/Orders/{id}` - Excluir pedido
- `WS /orderHub` - SignalR para notificações em tempo real

## 🎨 Design

- **Cores da Marca**: Azul (#2b5373), Vermelho (#e84c53), Cinza (#ececec)
- **UI Limpa**: Cards com sombras suaves, badges coloridos para status
- **Responsivo**: Funciona em iOS, Android e Web

## 👨‍💻 Desenvolvedor

**PHILLIPE ROGER SOUZA**

## 📝 Notas Técnicas

- **Result Pattern**: Todas as respostas da API seguem o padrão `{ data, isSuccess, errors }`
- **Clean Architecture**: Separação clara entre Service Layer, UI e State Management
- **SignalR**: Conexão WebSocket para notificações em tempo real
- **Atomic JSX**: Componentes atomizados para compatibilidade com editor visual
