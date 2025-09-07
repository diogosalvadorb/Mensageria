using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
            queue: "hello", 
            durable: false, 
            exclusive: false, 
            autoDelete: false,
            arguments: null);

Console.WriteLine("[x] Aguardando mensagens.");

// Cria um consumidor assíncrono que ficará "escutando" mensagens
var consumer = new AsyncEventingBasicConsumer(channel);

//DELEGATE - uma funcao que é executada quando recebe um evento ReceivedAsync                       
consumer.ReceivedAsync += (model, ea) =>
{
    //converte o corpo da mensagem para array de bytes
    var body = ea.Body.ToArray();

    // Decodifica os bytes para string (UTF-8)
    var message = Encoding.UTF8.GetString(body);

    // Desserializa a string JSON para um objeto Plane
    var plane = JsonSerializer.Deserialize<Plane>(message);

    Console.WriteLine($" [x] Mensagem recebida = {message}");

    // Retorna uma Task completada (padrão para métodos assíncronos de eventos)
    return Task.CompletedTask;
};

// Começa a consumir mensagens da fila "hello"
await channel.BasicConsumeAsync(
    queue: "hello",// Nome da fila
    autoAck: true,// true = confirma automaticamente o recebimento
    consumer: consumer);// consumer criado acima

Console.WriteLine("Pressione [enter] para sair.");
Console.ReadLine();

class Plane
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
