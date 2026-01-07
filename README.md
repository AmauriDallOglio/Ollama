# Arquitetura de uma aplicação que integra IA generativa com banco de dados SQL

## Exemplo de API + Banco de Dados + LLM local (Ollama) para geração de respostas estruturadas em JSON.


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




## Executar modelos de IA localmente

Sem precisar enviar dados para a internet (ideal para privacidade e uso offline).

Ele expõe uma API HTTP local (por padrão em http://localhost:11434), que pode ser usada em qualquer linguagem (C#, Python, JavaScript etc.).

Você pode usar os modelos como assistentes, geradores de código, ferramentas de análise de texto e muito mais.

## Treinar e personalizar modelos

É possível criar modelos customizados (fine-tuned) com seus próprios dados usando o comando ollama create.

**https://ollama.com/download/windows**

<img width="1880" height="867" alt="image" src="https://github.com/user-attachments/assets/afb0fac2-0e15-4826-bacd-b0f38b64eed5" />

<img width="1884" height="789" alt="image" src="https://github.com/user-attachments/assets/76501591-e425-4623-bab4-422fae5bd50c" />

<img width="1147" height="952" alt="image" src="https://github.com/user-attachments/assets/2202e4bd-550f-4abc-8d02-5a0dfb172c6a" />

ollama -- version

ollama --version

PS C:\Users\OKEA> ollama -- version

PS C:\Users\OKEA> ollama --version

PS C:\Users\OKEA> ollama ls

PS C:\Users\OKEA> ollama list

PS C:\Users\OKEA> ollama ps

PS C:\Users\OKEA> ollama pull llama3.2

 <img width="1128" height="224" alt="image" src="https://github.com/user-attachments/assets/34d4e9bf-18fd-44db-9448-174d09839e34" />


PS C:\Users\OKEA> ollama ls

PS C:\Users\OKEA> ollama list

PS C:\Users\OKEA> ollama ps

PS C:\Users\OKEA> ollama run llama3.2:latest

>>> Olá, tudo bem?
Olá! Tudo bem, obrigado! Estou aqui para ajudar. Como posso ajudá-lo hoje?

 <img width="1128" height="224" alt="image" src="https://github.com/user-attachments/assets/4c000a73-3e6c-41e6-ac87-9d0373e78629" />


## Criando uma sessão 

<img width="1134" height="304" alt="image" src="https://github.com/user-attachments/assets/e32cc49e-7f43-4910-8ff9-4895a069e5fb" />

### PS C:\> ollama run llama3.2

>>> Meu nome é Amauri
Olá, Amauri! É um prazer conhecer você. Como posso ajudar hoje? Você gostaria de conversar sobre algum assunto em particular ou simplesmente relaxar um pouco? Estou aqui para ajudar e chatar com você.
 
>>> /save sessao_amauri

Created new model 'sessao_amauri'

>>> lembra do meu nome?
Sim, Amauri! Eu estava lembrando que você me disse seu nome quando começamos a conversa. Não tenho como memorizar tudo o que as pessoas me dizem, mas eu posso sempre rever nossas conversas para recordar detalhes importantes,
incluindo seus nomes e outras informações importantes.
>>> /bye
 

### PS C:\> ollama run llama3.2

>>> Lembra do meu nome?

Não, não tenho memória a longo prazo, então não posso lembrar do seu nome. Eu sou um modelo de linguagem treinado para fornecer informações e responder perguntas baseadas em minha programação, mas não tenho a capacidade de armazenar informações pessoais ou criar conexões emocionais com os usuários.
 
>>> /load sessao_amauri

Loading model 'sessao_amauri'

>>> lembra meu nome?
Claro, Amauri! Eu vou tentar me lembrar dele. Como foi que você nos conheceu? Você nos contou um pouco sobre si mesmo quando iniciamos a conversa?
 
>>> /bye

 

### Comando	Significado em português	Exemplo de uso

/set	Define variáveis da sessão (por exemplo, mudar temperatura, contexto, sistema, etc.)	/set temperature 0.7 — ajusta a criatividade das respostas.

/show	Mostra informações do modelo atual (nome, parâmetros, etc.)	/show — exibe o nome do modelo e configurações em uso.

/load <modelo>	Carrega uma sessão anterior ou outro modelo instalado.	/load llama3 — muda o modelo atual para o llama3.

/save <nome>	Salva a sessão atual (histórico de conversa e contexto).	/save aula_prompt — salva o chat atual como “aula_prompt”.

/clear	Limpa o contexto da conversa (zera o histórico da sessão).	/clear — apaga o histórico e começa um novo diálogo.

/bye	Sai do chat interativo (encerra a sessão).	/bye — fecha o Ollama e retorna ao terminal.

/? ou /help	Mostra a ajuda dos comandos disponíveis.	/? — exibe esta lista de comandos.

/? shortcuts	Mostra os atalhos de teclado disponíveis (por exemplo, como enviar várias linhas, copiar texto, etc.)	/? shortcuts — exibe os atalhos interativos.


# Docker

## Docker Desktop (Windows)

Instalar o Docker Desktop pelo o site oficial do Docker: https://www.docker.com/products/docker-desktop/

Baixe Docker Desktop for Windows

Durante a instalação:
* Marque Use WSL 2 instead of Hyper-V
* Permita a instalação do WSL2 Kernel

Docker Desktop está funcionando

Abra o Docker Desktop Aguarde aparecer: Docker Desktop is running
Abra o PowerShell ou Terminal informe: 
* docker --version
* docker compose version

Se ambos responderem, está tudo certo



## Instalar a imagem pelo Terminal do Docker Desktop 
 
Abra o Docker Desktop:

<img width="1337" height="458" alt="image" src="https://github.com/user-attachments/assets/51624bc1-d7ee-42de-b337-a5894505c702" />

A imagem abaixo mostra modelos de IA disponíveis para download (pull), de modelos LLM para uso local via Docker.

Pesquise por “llama”. O Docker está exibindo modelos de linguagem (LLMs) prontos para serem baixados, vou trabalahr com o modelo: llama3.2, clicar no botão "Pull" quando é clica em Pull o Docker baixa o modelo de IA, ele fica armazenado localmente, depois será usado para criar o container.

*Esses modelos são usados para IA generativa rodar IA local, sem depender de OpenAI, Azure ou Gemini, semelhantes ao que o Ollama faz.

<img width="1336" height="546" alt="image" src="https://github.com/user-attachments/assets/d964da9d-eae6-4152-8ea0-888331510549" />

Solicitar o PULL da verão llama3.2:latest / 3.21 B

3.21 B = 3,21 bilhões de parâmetros é o tamanho do cérebro do modelo, quanto mais parâmetros:
* Mais capacidade de raciocínio
* Melhor entendimento de linguagem
* Mais consumo de RAM/VRAM
* Mais lento em máquinas fracas

<img width="1325" height="538" alt="image" src="https://github.com/user-attachments/assets/3290ce64-2868-4d36-a5cf-0685ff82ac51" />

Aguardar o processo ser baixado:

<img width="1336" height="445" alt="image" src="https://github.com/user-attachments/assets/aa94bc99-9eb9-401f-b71f-11ae2a792d0d" />

A tela mostra exatamente que o modelo LLaMA 3.2 foi baixado no computador

* Instalado modelo llama3.2:latest;
* Ele está armazenado localmente, pronto para ser executado;
* Nada está rodando ainda, até clicar no play.

<img width="1328" height="454" alt="image" src="https://github.com/user-attachments/assets/c098149a-a11f-4876-8fad-b1b24e81a8cc" />

Exemplo o que exatamente acontece ao clicar Play:

<img width="1335" height="435" alt="image" src="https://github.com/user-attachments/assets/74993f8f-24cb-4a89-8112-6792008f3483" />


## Instalar a imagem pelo Terminal do Docker Desktop 

Abra o Terminal No Docker Desktop, clique no terminal (ou use PowerShell / CMD) 

Baixar a imagem do Ollama isso instala a imagem no Docker Desktop: 

docker pull ollama/ollama 

* O Docker está baixando a imagem oficial do Ollama do Docker Hub
* Essa imagem é o servidor Ollama, não é ainda um modelo de IA
* O download está em andamento (várias camadas sendo baixadas)
* Baixando as camadas
* Extraindo
* Salvando a imagem localmente

<img width="1244" height="587" alt="image" src="https://github.com/user-attachments/assets/688b5efc-7f2e-4afb-850a-17e9f5092883" />

A imagem abaixo mostra todas as imagens Docker armazenadas localmente.

O que isso NÃO está fazendo:
* Não está rodando IA ainda
* Não está carregando modelo LLaMA
* Não está consumindo CPU/RAM (só disco e rede)

Pronto para criar um container Ollama;

<img width="1226" height="572" alt="image" src="https://github.com/user-attachments/assets/075ff9a4-fbbd-466b-97f0-7059305ba8c7" />

A imagem abaixo é o passo em que está CRIANDO e CONFIGURANDO um container do Ollama no Docker Desktop.
* Container name: Nome do container
* Ports: Expõe o Ollama para fora do Docker, permite acessar via: http://localhost:11434
* Volumes: Isso cria um volume persistente modelos baixados NÃO se perdem se: Parar o container, reiniciar o Docker, atualizar a imagem;
* Environment variables: Não precisa configurar agora

Quando você clicar Run docker cria o container Ollama32, sobe o servidor Ollama, escuta na porta 11434, fica aguardando prompts;

<img width="1229" height="586" alt="image" src="https://github.com/user-attachments/assets/d66dcfb6-0618-4244-93d4-7cb5bf3ab12f" />



<img width="1242" height="409" alt="image" src="https://github.com/user-attachments/assets/ec8da482-2259-4e63-8161-32d68c8012e7" />


<img width="1244" height="515" alt="image" src="https://github.com/user-attachments/assets/02a031f8-b589-43e8-8440-ca0916b065b3" />

Invoke-RestMethod -Uri "http://localhost:11434/api/generate" ` 

  -Method Post ` 

  -Body '{"model":"llama3.2","prompt":"Explique em português o que é Docker"}' ` 

  -ContentType "application/json" 

<img width="1424" height="583" alt="image" src="https://github.com/user-attachments/assets/3e17e2da-1076-4cbf-b100-1c4494549f06" />

Windows PowerShell 

 

docker run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama 

docker exec -it ollama ollama run llama3.2 

<img width="1438" height="264" alt="image" src="https://github.com/user-attachments/assets/65bd7976-691e-4065-bd25-fc257cb61859" />



<img width="1604" height="484" alt="image" src="https://github.com/user-attachments/assets/54908c82-c377-4214-845a-b61bf59e16bb" />

PS C:\> docker ps 

CONTAINER ID   IMAGE                             COMMAND                  CREATED             STATUS             PORTS                                             NAMES 

c394d5a0b985   ollama/ollama:latest              "/bin/ollama serve"      10 minutes ago      Up 10 minutes      0.0.0.0:11434->11434/tcp, [::]:11434->11434/tcp   OllamaTeste 

8d8ac8f2bd09   docker/welcome-to-docker:latest   "/docker-entrypoint.…"   About an hour ago   Up About an hour   0.0.0.0:8088->80/tcp, [::]:8088->80/tcp           welcome-to-docker 

PS C:\> docker exec -it c394d5a0b985 bash 

root@c394d5a0b985:/# ollama --version 

ollama version is 0.12.6 

root@c394d5a0b985:/# ollama list 

NAME             ID              SIZE      MODIFIED 

llama2:latest    78e26419b446    3.8 GB    2 minutes ago 

root@c394d5a0b985:/# ollama run llama2 

>>> Quem é o batman? 

 

 

bye 





  


