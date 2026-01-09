# Arquitetura de uma aplicação que integra IA generativa com banco de dados SQL

## Exemplo de API + Banco de Dados + LLM local (Ollama) e Docker para geração de respostas estruturadas em JSON.


<img width="933" height="615" alt="image" src="https://github.com/user-attachments/assets/c6db6cea-8f80-4efc-b08f-6b378793e3e1" />

### 1 Usuário faz uma requisição para a API

### 2 A API: Consulta o SQL Server + Monta um prompt com dados do banco;
* Prompt é simplesmente o texto de entrada que você envia para um modelo de IA, no exemplo é a instrução + pergunta + dados que dizem à IA o que ela deve fazer.
* O banco NÃO conversa direto com o Ollama
* Quem orquestra tudo é a sua API
  
### 3 O prompt generativo é enviado para o Ollama
* Um prompt generativo é um prompt estruturado, feito para gerar conteúdo alinhado ao negócio. Nos exemplos ele normalmente contém: Contexto, Instrução clara, Dados, Formato de saída esperado;
* Ollama é uma plataforma local que permite rodar modelos de IA generativa (LLMs) no seu computador ou servidor, sem depender de cloud (OpenAI, Azure, etc) motor local que executa IA generativa.

### 4 A IA Generativa processa o prompt
* IA Generativa é um tipo de Inteligência Artificial a tecnologia capaz de: Gerar texto, gerar código, gerar resumos, gerar explicações, gerar respostas baseadas em contexto

### 5 A API recebe a resposta
* Orquestradora de dados + prompt + resposta

### 6 A API retorna JSON estruturado para o serviço

Detalhes:

https://github.com/AmauriDallOglio/Wiki/wiki/Ollama



