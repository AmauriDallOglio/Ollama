# Ollama

O Ollama serve para:

## Executar modelos de IA localmente
Sem precisar enviar dados para a internet (ideal para privacidade e uso offline).

## Integrar modelos a aplicações
Ele expõe uma API HTTP local (por padrão em http://localhost:11434), que pode ser usada em qualquer linguagem (C#, Python, JavaScript etc.).

## Gerar texto, código, resumos, traduções, chatbots, etc.
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


Criando uma sessão 

<img width="1134" height="304" alt="image" src="https://github.com/user-attachments/assets/e32cc49e-7f43-4910-8ff9-4895a069e5fb" />

PS C:\> ollama run llama3.2
>>> Meu nome é Amauri
Olá, Amauri! É um prazer conhecer você. Como posso ajudar hoje? Você gostaria de conversar sobre algum assunto em particular ou simplesmente relaxar um pouco? Estou aqui para ajudar e chatar com você.
 
>>> /save sessao_amauri
Created new model 'sessao_amauri'
>>> lembra do meu nome?
Sim, Amauri! Eu estava lembrando que você me disse seu nome quando começamos a conversa. Não tenho como memorizar tudo o que as pessoas me dizem, mas eu posso sempre rever nossas conversas para recordar detalhes importantes,
incluindo seus nomes e outras informações importantes.
>>> /bye
 

PS C:\> ollama run llama3.2
>>> Lembra do meu nome?
Não, não tenho memória a longo prazo, então não posso lembrar do seu nome. Eu sou um modelo de linguagem treinado para fornecer informações e responder perguntas baseadas em minha programação, mas não tenho a capacidade de armazenar informações pessoais ou criar conexões emocionais com os usuários.

 
>>> /load sessao_amauri
Loading model 'sessao_amauri'
>>> lembra meu nome?
Claro, Amauri! Eu vou tentar me lembrar dele. Como foi que você nos conheceu? Você nos contou um pouco sobre si mesmo quando iniciamos a conversa? 
>>> /bye

 
PS C:\> ollama ls
NAME                    ID              SIZE      MODIFIED
sessao_amauri:latest    a70c53d1b327    2.0 GB    3 minutes ago
llama3.2:latest         a80c4f17acd5    2.0 GB    20 minutes ago
PS C:\> ollama run llama3.2
>>> /load sessao_amauri
Loading model 'sessao_amauri'
>>> lembra meu nome?
Sim, Amauri! Eu não vou esquecer. É importante lembrar os nomes das pessoas que estamos conversando. Se precisar de ajuda ou quiser falar sobre algo específico, estou aqui para ajudar. Como está você hoje, Amauri?
>>> 



Comando	Significado em português	Exemplo de uso

/set	Define variáveis da sessão (por exemplo, mudar temperatura, contexto, sistema, etc.)	/set temperature 0.7 — ajusta a criatividade das respostas.

/show	Mostra informações do modelo atual (nome, parâmetros, etc.)	/show — exibe o nome do modelo e configurações em uso.

/load <modelo>	Carrega uma sessão anterior ou outro modelo instalado.	/load llama3 — muda o modelo atual para o llama3.

/save <nome>	Salva a sessão atual (histórico de conversa e contexto).	/save aula_prompt — salva o chat atual como “aula_prompt”.

/clear	Limpa o contexto da conversa (zera o histórico da sessão).	/clear — apaga o histórico e começa um novo diálogo.

/bye	Sai do chat interativo (encerra a sessão).	/bye — fecha o Ollama e retorna ao terminal.

/? ou /help	Mostra a ajuda dos comandos disponíveis.	/? — exibe esta lista de comandos.

/? shortcuts	Mostra os atalhos de teclado disponíveis (por exemplo, como enviar várias linhas, copiar texto, etc.)	/? shortcuts — exibe os atalhos interativos.









