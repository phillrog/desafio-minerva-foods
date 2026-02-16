# [![CI/CD Desafio Minerva Foods - Backend](https://github.com/phillrog/desafio-minerva-foods/actions/workflows/deploy.yml/badge.svg)](https://github.com/phillrog/desafio-minerva-foods/actions/workflows/deploy.yml) - [![CI/CD Desafio Minerva Foods - Frontend](https://github.com/phillrog/desafio-minerva-foods/actions/workflows/expo-deploy.yml/badge.svg)](https://github.com/phillrog/desafio-minerva-foods/actions/workflows/expo-deploy.yml) - [<img src="https://img.shields.io/badge/Railway%20Deploy-black?style=for-the-badge&logo=railway" width="220" height="38.45"/></a>](https://desafio-mater-imperium-production.up.railway.app/swagger-ui/index.html)

# 📦 Desafio Minerva Foods 

Este repositório contém a solução completa para o desafio técnico de gestão de pedidos, abrangendo uma API robusta de alta performance e um aplicativo móvel intuitivo.

<img width="704" height="1472" alt="Gemini_Generated_Image_kmbbfzkmbbfzkmbb" src="https://github.com/user-attachments/assets/b6d84530-80c7-4fa6-8e0f-2b8942bc0f70" />



🚀 Tecnologias
----------------------------

### **Front-end**

-   **Framework:** React Native com **Expo** (SDK 52).

-   **Linguagem:** TypeScript.

-   **Roteamento:** Expo Router (File-based routing).

-   **Ícones:** Lucide React Native.

-   **Segurança:** Armazenamento seguro de **JWT** para autenticação.

-   **SignalR**: Conexão WebSocket para receber notificações em tempo real

### **Back-end (API)**

-   **Plataforma:** .NET 8 / C#.

-   **Arquitetura:** **Clean Architecture** com foco em **Domain-Driven Design (DDD)**.

-   **Padrões:** **CQRS** (Command Query Responsibility Segregation) para separação de leitura e escrita.

-   **Mensageria:** **RabbitMQ** para processamento 100% assíncrono (Delivery, Cadastro e Aprovação).

-   **Persistência:** Entity Framework Core 

-   **Segurança:** Autenticação e Autorização via **JWT**.

-   **Observabilidade:** Rastreabilidade e logs de transações.

-   **Documentação:** Swagger (OpenAPI).

-   **SignalR**: Conexão WebSocket para disparar notificações em tempo real


* * * * *

🛠️Arquitetura
------------------------------------------

### **1\. Clean Architecture & SOLID**

A aplicação foi dividida em camadas lógicas (Domain, Application, Infrastructure e WebAPI) para garantir que as regras de negócio sejam independentes de frameworks externos. O uso de **SOLID** permitiu uma base de código extensível e de fácil manutenção.

### **2\. Processamento Assíncrono com RabbitMQ**

Diferente de uma simulação em memória, implementamos um broker real. O fluxo de criação de pedido dispara eventos que são consumidos por workers dedicados:

-   **Fila de Delivery:** Calcula prazos de entrega de forma isolada.

-   **Fila de Cadastro:** Garante a persistência do pedido.

-   **Fila de Aprovação:** Processa a transição de status de forma segura.

### **3\. CQRS & Behaviors**

Utilizado o MediatR para implementar o CQRS com **Pipelines/Behaviors** para:

-   **Global Exception Handling:** Captura de erros centralizada.

-   **Result Pattern:** Todas as respostas da API seguem um contrato único (Result Pattern).

-   **Unit of Work:** Garantia de atomicidade em operações de escrita.

### **4\. Segurança e Rastreabilidade**

Além do JWT, foi implementeado a passagem do contexto do usuário da requisição para dentro das mensagens da fila, permitindo gravar "quem fez o quê" mesmo em processos assíncronos (Audit Log). As entidades foram desenhadas com foco em **imutabilidade**.

* * * * *

🧪 Qualidade e Testes
---------------------

-   **Testes Unitários:** Implementados para as regras de negócio críticas (Cálculo de status baseado em valor, validações de pedidos).

-   **Docker Compose:** Ambiente conteinerizado para subir a API, Banco de Dados e RabbitMQ com um único comando.

* * * * *


## 🚀 Funcionalidades

- **Autenticação**: Login com usuário e senha conectado à API local
- **Listagem de Pedidos**: Visualização de todos os pedidos com status coloridos
- **Detalhes do Pedido**: Informações completas de cada pedido
- **Criação de Pedidos**: Formulário para criar novos pedidos
- **Aprovação de Pedidos**: Aprovar pedidos que requerem aprovação manual
- **Notificações em Tempo Real**: SignalR para atualizações instantâneas
- **Status Numéricos**: Sistema de status baseado em enum (0-9)


## 🔌 API Endpoints

A aplicação se conecta aos seguintes endpoints em `http://localhost:5001`:

- `POST /api/auth/login` - Autenticação
- `GET /api/Orders` - Lista de pedidos
- `GET /api/Orders/{id}` - Detalhes do pedido
- `POST /api/Orders` - Criar pedido
- `PUT /api/Orders/{id}/approve` - Aprovar pedido
- `GET /api/Customer` - Lista de clientes
- `GET /api/PaymentCondition` - Lista de formas de pagamentos
- `WS /orderHub` - SignalR para notificações em tempo real


📱 Como Executar
----------------

### **Back-end**

Bash

```
docker-compose up -d

```

Acesse o Swagger em: `http://localhost:5001/swagger`

### **Front-end**

O app está publicado via **EAS Update** para teste imediato:

1.  Instale o **Expo Go** no seu celular.

2.  Acesse o link 

- https://expo.dev/preview/update?message=Fim&updateRuntimeVersion=1.0.0&createdAt=2026-02-16T17%3A18%3A41.147Z&slug=exp&projectId=9879c7fa-8991-4084-9841-5379b8b29494&group=1d4f7125-28ba-46db-9a0e-b622926dcf92
- exp+://expo-development-client/?url=https://u.expo.dev/9879c7fa-8991-4084-9841-5379b8b29494/group/1d4f7125-28ba-46db-9a0e-b622926dcf92

3.  Escaneie o QR Code.

<img width="612" height="727" alt="image" src="https://github.com/user-attachments/assets/d31cf49a-034f-483f-b703-2952f2e70b5f" />


Exemplo:

![dcm2](https://github.com/user-attachments/assets/fb32a0ce-54cc-4033-aa56-c752740299a0)

4. Executar localhost

```
npm install --legacy-peer-deps

npx expo start -c
```

Abrir em http://localhost:8081/

* * * * *

# Resultado

## APP

### Login

<img width="510" height="1079" alt="Captura de tela 2026-02-16 133713" src="https://github.com/user-attachments/assets/33a6554a-a701-42c1-8ade-1f03792f3d1a" />

### Listagem de pedidos

<img width="509" height="1079" alt="Captura de tela 2026-02-16 133755" src="https://github.com/user-attachments/assets/5d354add-872f-4982-8292-56727eb624fb" />

### Novo pedido

<img width="518" height="1079" alt="Captura de tela 2026-02-16 133817" src="https://github.com/user-attachments/assets/e9b11d28-a491-4aeb-a1b3-e4c6a21b0f0f" />

### Detalhes do pedido

<img width="505" height="1079" alt="Captura de tela 2026-02-16 133910" src="https://github.com/user-attachments/assets/5ef9a834-b873-4cb1-b422-62b84de3da41" />

### Perfil/Sair

<img width="517" height="1079" alt="Captura de tela 2026-02-16 133944" src="https://github.com/user-attachments/assets/069cab48-a1ed-41a4-a63f-893843fa2b1e" />


## API
![dcm](https://github.com/user-attachments/assets/befc607d-127c-49ea-9cbb-0897e4599965)




❤️ para Sara 💙
