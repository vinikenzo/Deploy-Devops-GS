# 🌍 EcoOrbit — Deploy DevOps (GS 2026/1)

Pipeline de CI/CD do componente **.NET** do **EcoOrbit**, plataforma de monitoramento ambiental para detecção de riscos de incêndio e desmatamento. Este repositório contém a aplicação ASP.NET Core containerizada e o pipeline Azure DevOps responsável pelo build e push automático da imagem Docker para o **Azure Container Registry (ACR)**.

---

## 👥 Equipe

| Nome | RM |
|---|---|
| Vinícius Kenzo | RM 559982 |
| João Victor Alves da Silva | RM 559726 |  
| Lucas Gomes de Araújo Lopes | RM 559607 |

**Turma:** 2TDSPA — FIAP  
**Disciplina:** DevOps Tools & Cloud Computing — GS 2026/1

---

## 🏗️ Arquitetura do Repositório
Deploy-Devops-GS/
├── ecoorbit-dotnet-fixed/          # Aplicação ASP.NET Core
│   └── src/
│       └── ecoorbit-dotnet/
│           └── Dockerfile          # Imagem Docker da aplicação
└── azure-pipelines.yml             # Pipeline de CI/CD (Azure DevOps)

---

## ⚙️ Pipeline CI/CD — Azure DevOps

O arquivo `azure-pipelines.yml` define o pipeline de integração e entrega contínua.

**Trigger:** branch `Teste-EcoOrbit`  
**Agent Pool:** `ubuntu-latest`

### Estágios

#### 🔨 Build e Push para ACR

| Campo | Valor |
|---|---|
| Task | `Docker@2` |
| Container Registry | `dotnetmssqlrm559982` (Service Connection) |
| Repository (ACR) | `dotnetsql` |
| Dockerfile | `ecoorbit-dotnet-fixed/src/ecoorbit-dotnet/Dockerfile` |
| Build Context | `ecoorbit-dotnet-fixed/` |
| Tags geradas | `$(Build.BuildId)` e `latest` |

O pipeline executa `buildAndPush` automaticamente a cada push na branch de trigger, gerando uma imagem versionada pelo ID do build e atualizando a tag `latest`.

---

## 🐳 Docker

A aplicação é containerizada via Dockerfile localizado em `ecoorbit-dotnet-fixed/src/ecoorbit-dotnet/Dockerfile`.

Para build local (a partir da raiz do repositório):

```bash
docker build -f ecoorbit-dotnet-fixed/src/ecoorbit-dotnet/Dockerfile \
             -t ecoorbit-dotnet:local \
             ./ecoorbit-dotnet-fixed
```

---

## 🚀 Como acionar o pipeline

1. Faça push de alterações para a branch `Teste-EcoOrbit`:

```bash
git checkout Teste-EcoOrbit
git add .
git commit -m "feat: sua alteração"
git push origin Teste-EcoOrbit
```

2. O Azure DevOps detecta o push, executa o stage **Build e Push para ACR** e publica a imagem no registry `dotnetmssqlrm559982`.

---

## 🛠️ Tecnologias

- **ASP.NET Core** (C#) — API da plataforma EcoOrbit
- **Docker** — containerização da aplicação
- **Azure DevOps Pipelines** — CI/CD automatizado
- **Azure Container Registry (ACR)** — armazenamento das imagens Docker

---

## 🌐 Contexto — EcoOrbit

O EcoOrbit é uma plataforma de monitoramento ambiental que utiliza imagens de satélite (NASA GIBS API) e modelos de Machine Learning para identificar riscos de incêndio e desmatamento em tempo real. Este repositório cobre exclusivamente o componente .NET e sua esteira de deploy.

Demais componentes do projeto:

| Componente | Tecnologia |
|---|---|
| Backend principal | Java Spring Boot (microserviços) |
| Serviço de ML | Python Flask |
| Mobile | React Native |
| Banco de dados | Oracle / PostgreSQL |

---
