using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

//cria uma conexão que se conectar ao RabbitMQ
var factory = new ConnectionFactory() { HostName = "localhost" };

//abre uma conexão assicrona com o servidor rabbitMQ(versão 7.1.2)
//cria um canal de comunicação dentro da conexao (declarar, consumir, publicar fila) são operações que são feitas dentro do canal
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

//declara uma fila chamada hello, se a fila já existente não faz nada
await channel.QueueDeclareAsync(
            queue: "hello",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

Console.WriteLine("Digite uma mensagem e presione Enter");

while (true)
{
    var message = Console.ReadLine();

    //declara um objeto e serializa para json
    var plane = new Plane { Id = 1, Name = "Boeing 737" };
    message = JsonSerializer.Serialize(plane);

    if (string.IsNullOrEmpty(message)) break;
    //converte a mensagem(string) para um array de bytes, pois o RabbitMQ trabalha com array de bytes
    var body = Encoding.UTF8.GetBytes(message);

    // Publica a mensagem na fila "hello"
    await channel.BasicPublishAsync(
                exchange: string.Empty, //Usa o Default Exchange
                routingKey: "hello", // Nome da fila onde vai ser publicada a mensagem
                body: body); // Corpo da mensagem em bytes

    Console.WriteLine($" [x] Enviado {message}");
}

class Plane
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
