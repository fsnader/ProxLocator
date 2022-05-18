# ProxLocator

Sistema de Localização em Tempo Real baseado em RFID, desenvolvido como projeto de final de curso de Engenharia de Controle e Automação na UFMG.

- [Monografia Completa](../main/docs/monografia.pdf)
- [Apresentação]("../../main/docs/apresentacao.pdf")


## Arquitetura

O software de localização desenvolvido neste trabalho foi batizado de Prox!Locator e possuía o objetivo de calcular a posição de uma tag móvel através do recebimento de pacotes UDP dos checkpoints. Para fazer isso, eram suas responsabilidades receber os pacotes, organizar os dados recebidos e executar o cálculo da posição.

A fim de garantir a escalabilidade do sistema e possibilitar alterações como a adição de antenas e tags, troca dos modelos de cálculo e inclusão de regras de negócio adicionais, o Prox!Locator foi organizado em componentes, conforme apresentado na figura 4.1 e com as seguintes responsabilidades:

1. Recebimento de pacotes UDP, gerenciamento de conexões e envio dos pacotes recebidos para os componentes responsáveis por sua interpretação;
2. Tradução das mensagens recebidas, organização dos valores de RSSI por tag/antena, filtragem e envio dos dados no formato correto para cálculo da posição;
3. Cálculo da posição do tag através dos valores de RSSI obtidos; 
4. Apresentação dos dados recebidos e/ou processados para o usuário, através de uma interface gráfica.

![image](https://user-images.githubusercontent.com/42560361/168935771-b0ab472b-4a10-4969-81c2-2e8587ece997.png)

A principal tecnologia escolhida para o desenvolvimento do sistema foi o .NET Framework e a linguagem C#,por sua robustez, grande variedade de bibliotecas disponíveis e consolidação no mercado. O framework apresenta bibliotecas nativas para uso de sockets UDP, estruturas de dados, gerenciamento de threads, entre outras funcionalidades necessárias durante o desenvolvimento.

A interface gráfica utiliza o Windows Presentation Foundation (WPF), um framework de desenvolvimento de interfaces gráficas contido no .NET Framework. A plataforma é largamente utilizada para o desenvolvimento de aplicações para Windows e compartilha das vantagens do .NET Framework.

Devido às pesquisas feitas na fase de blueprint, as redes neurais artificiais foram escolhidas como base do modelo de localização do projeto. Embora existam bibliotecas para o design, teste e execução de redes neurais no .NET Framework, outras tecnologias apresentam melhores recursos e são mais recomendadas para esse tipo de análise. Dessa forma, a modelagem inicial das redes neurais utilizadas no projeto é feita utilizando as bibliotecas de Redes Neurais do MATLAB e a implementação e treinamento dessas redes é feita utilizando a linguagem Python. Por fim, os modelos de redes neurais desenvolvidos no Python podem ser exportados e implementados dentro do .NET Framework, através do uso da biblioteca NNSharp.

## Desenvolvimento

Os componentes do sistema foram especificados através de interfaces. Interfaces são contratos que definem quais os métodos e propriedades das classes sem definir sua implementação. Essa abordagem fornece algumas vantagens, pois permite que componentes que implementem a mesma interface sejam substituídos entre si sem que precisem ser refatorados.

Já as classes concretas são implementações das interfaces, contendo toda a lógica necessária para fornecer os métodos e propriedades esperadas por ela. Na figura abaixo são apresentadas as interfaces e as implementações concretas dos componentes previstos na arquitetura. Dessa forma, cada componente criado durante o projeto não só pode ser utilizado separadamente como pode ser reutilizado dentro da própria aplicação.

![image](https://user-images.githubusercontent.com/42560361/168936171-a999e2ea-b7fa-4051-b2a8-ecab0b3de737.png)

## Recebimento de pacotes UDP

O primeiro componente do sistema é o receptor de pacotes, responsável por criar um cliente UDP, receber as mensagens e enviar os pacotes recebidos para um consumidor de pacotes. Esse componente recebe como entradas a porta e onde o cliente UDP deverá ser executado e o consumidor de dados. Essa camada foi desenvolvida dentro de uma biblioteca chamada de [XSockets](https://github.com/fsnader/XSockets), que segue o seguinte pseudocódigo:

1. Recebe um objeto do tipo consumidor (que implemente a interface ISocketConsumer) e o número da porta que será utilizada pelo Socket no seu construtor
2. Inicia um cliente UDP na porta especificada
3. Aguarda por novas mensagens
4. Chama o método Notify() do componente Consumidor sempre que uma nova mensagem for recebida

## Recebimento de pacotes UDP

O primeiro componente do sistema é o receptor de pacotes, responsável por criar um cliente UDP, receber as mensagens e enviar os pacotes recebidos para um consumidor de pacotes. Esse componente recebe como entradas a porta e onde o cliente UDP deverá ser executado e o consumidor de dados. Essa camada foi desenvolvida dentro de uma biblioteca chamada de *XSockets*, que segue o seguinte pseudocódigo:

-  Recebe um objeto do tipo consumidor (que implemente a interface *ISocketConsumer*) e o número da porta que será utilizada pelo *Socket*no seu construtor
-  Inicia um cliente UDP na porta especificada
-  Aguarda por novas mensagens
-  Chama o método *Notify()* do componente Consumidor sempre que uma nova mensagem for recebida.


## Consumidor de pacotes
O segundo componente é o consumidor que deverá ser injetado dentro do receptor de pacotes. Esse componente implementa a interface *ISocketConsumer* e é responsável por tratar os pacotes recebidos pelo cliente UDP.

### Interface: *ISocketConsumer*
A interface *ISocketConsumer* exige que os consumidores implementem os seguintes métodos:

-  SetSocket(Socket socket): Método utilizado para criar um canal de comunicação entre a aplicação e o *endpoint* que enviou men
-  Notify(byte[] message, IPEndPoint endpoint): Método chamado toda vez que uma nova mensagem binária é recebida pelo clientUDPro desse método deverá ser implementada toda a lógica de processamento dos dados recebidos.


### Implementação Concreta: ProxLocatorConsumer

A implementação concreta do consumidor foi feita na classe *ProxLocatorConsumer, representada pelo diagrama de classes abaixo. Em seu método *Notify*, o consumidor decodifica o pacote binário recebido e cria uma instância de um objeto de mensagem, contendo o identificador da *tag*, o IP do *checkpoint* que recebeu a mensagem e o valor de RSSI recebido. Em seguida, ele envia esse pacote decodificado para o próximo componente, o processador, através do método *ProcessMessage*.

![image](https://user-images.githubusercontent.com/42560361/168937384-8539bdd6-ed06-4cbc-b9da-ba8c0ccb620d.png)


## Processador

O terceiro componente é o processador que deverá ser injetado dentro do Consumidor de pacotes. Esse componente implementa a interface *IProcessor* e é o responsável por todo o processamento lógico das mensagens para obtenção da posição, como a separação das mensagens em filas por *checkpoint* e *tag*, filtragem, descarte de mensagens inapropriadas, organização dos *datasets*, chamada do mecanismo de cálculo  e disponibilização da última posição obtida.

### Interface: IProcessor

A interface *IProcessor* exige que os processadores implementem os seguintes métodos e propriedades:

- LastPosition: Propriedade que indica a última posição calculada, com a data/horário de cálculo e posições x e y;
- ProcessMessage(LocatorMessage message): Método chamado toda vez que uma nova mensagem é traduzida no consumidor. Esse método é a entrada lógica para todo o processamento feito nessa etapa.


### Implementação Concreta: MessagesProcessor
O MessagesProcessor é chamado toda vez que uma nova mensagem é recebida e deve ser capaz de processar essas mensagens independentemente da ordem de chegada. Para isso, ele conta com uma estrutura de dados interna para armazenar as mensagens recebidas em filas.

Essa estrutura, apresentada no diagrama de classes da figura abaix consiste em uma lista de objetos do tipo *Checkpoint*. Cada *checkpoint* encapsula uma lista de *Tags* e possui métodos para enfileiramento e desenfileiramento de mensagens. Cada *tag* possui uma fila para armazenar as mensagens recebidas e um filtro. Assim, cada mensagem recebida é filtrada e armazenada em uma fila referente ao seu par *tag/checkpoint.

![image](https://user-images.githubusercontent.com/42560361/168937415-4e5ec6b6-924e-4253-856f-ccf8c907cd0c.png)

O filtro é implementado a partir da interface *IFilter* e deve possuir um método para filtragem e outro para limpeza do *buffer* interno. Para o projeto do Prox!Locator, foram implementados um filtro de médias móveis, um filtro de medianas e um filtro de médias móveis das medianas, composto dos dois anteriores.

Na figura abaixo é possível ver a classe principal do *MessageProcessor*, com os métodos e propriedades definidas nela. Conforme as mensagens chegam e são enfileiradas, o processador verifica se existem dados suficientes para fazer uma previsão através dos métodos privados *RefreshDataSet, EnqueueCurrentRow e RefreshCurrentDataRow*.

Se existirem valores de RSSI do mesmo momento para todas as *tags* e *checkpoints*, esses dados são enviados para o estimador de posição através do método *CalculatePosition*, que retorna a posição calculada. Essa posição fica disponível na propriedade pública *LastPosition*, podendo ser acessada por componentes externos (como a interface do usuário).

![image](https://user-images.githubusercontent.com/42560361/168937443-93819306-677b-4210-936c-73d3a4dcf0d0.png)


## Estimador de Posição

O estimador de posição é o componente responsável por calcular a posição da *tag* através de um vetor de mensagens recebidas. A técnica utilizada e os dados necessários nesse vetor dependem da implementação escolhida.

### Interface: IPositionProvider
A interface IPositionProvider define o seguinte método:

-  GetPosition(LocationMessage[] dataRow): Método que recebe um vetor de mensagens de posição e retorna a posição calculada.

### Implementação Concreta: NeuralNetPositionProvider

Durante o capítulo de Caracterização dos Modelos, diferentes modelos de posicionamento foram avaliados. No fim dos experimentos, foi constatado que o modelo de topologia 4 apresentava o melhor desempenho para o problema proposto. Assim, esse modelo foi implementado dentro do Estimador do Posição do ProxLocator, através de redes neurais.

Primeiramente, os modelos foram montados e treinados utilizando a linguagem *Python* e a biblioteca *Keras*. Em seguida foram exportados para um arquivo de extensão *json* e posteriormente importados dentro do código C#, com o uso da biblioteca *NNSharp*.

Dentro da classe *NeuralNetPositionProvider*, representado pelo diagrama de classes da figura abaixo, as redes neurais importadas foram encapsuladas em objetos do tipo *NeuralNetwork*. Ao receber os dados de RSSI através do método *GetPosition*, o estimador executa o cálculo da posição conforme o algoritmo definido para o modelo:


-  Calcula a probabilidade da distância ser maior que 3.5 metros através da rede neural de classificação (*distanceClassificationModel*);
-  Se a probabilidade for menor que 0.35, é utilizada a rede neural treinada para distâncias menores que 3.5 metros (*shortDistanceModel*);
-  Se a probabilidade tiver valor entre 0.35 e 0.65, é utilizada uma rede neural treinada para todas as distâncias (*anyDistanceModel*);
-  Se a probabilidade tiver valor maior que 0.65, é utilizada uma rede neural treinada para distâncias maiores que 3.5 metros(*longDistanceModel*).

![image](https://user-images.githubusercontent.com/42560361/168937513-ac294481-6b83-4c7b-8bd7-689a92927e07.png)


## Interface Gráfica

A interface gráfica desenvolvida para o Prox!Locator foi feita utilizando o *Windows Presentation Foundation (WPF)* com o objetivo de não só apresentar a posição em tempo real das *tags*, mas também de controlar a execução do mecanismo de recebimento de pacotes / processamento da posição.

A estrutura da aplicação gráfica foi baseada no padrão de arquitetura MVVM (*Model View ViewModel*), que tem o objetivo de separar a lógica da aplicação da interface gráfica implementada. Assim, foram criadas duas classes: 


-  MainWindow.cs/MainWindow.xaml: Classe onde todos os elementos gráficos da tela são definidos e organizados através de um arquivo no padrão XAML.
-  ProxLocatorViewModel: Classe onde todo o processamento de dados é feito e os valores que serão exibidos na tela são disponibilizados.


Ao criar a tela na classe MainWindow, o *framework* WPF permite que uma propriedade chamada *DataContext* seja definida. Assim, essa propriedade é definida como o *ProxLocatorViewModel*, e a aplicação faz a conexão lógica entre as propriedades da classe e a interface gráfica.

Na interface gráfica, apresentada na figura abaixo, são exibidos os seguintes elementos:

-  Um gráfico apresentado a posição dos *checkpoints*, *tags* fixas, localização real da *tag* móvel (no caso de simulação) e sualocalização estimada;
-  Um cartão com as coordenadas estimadas da *tag* e a data/hora do último cálculo feito;
-  Um botão de início e parada do cálculo de posição;
-  Uma chave para seleção do modo de simulação


Caso o modo de simulação esteja desligado, o programa irá iniciar o receptor de pacotes UDP e todos os componentes de processamento de mensagens e cálculo de posição. Caso o modo de simulação esteja ativado, os componentes de processamento serão inicializados sem o receptor de pacotes UDP e os dados de RSSI serão obtídos de um arquivo de texto.

Já os botões de iniciar/parar a aplicação são utilizados para iniciar e interromper o processamento das mensagens, que tem os resultados exibidos em tempo real tanto no gráfico de posição quanto no card de resultados.

## Estrutura do Projeto
TODO
