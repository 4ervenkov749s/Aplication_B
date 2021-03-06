using Aplication_B.BL.Kafka;
using MessagePack;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Aplication_B.BL
{
    public class PersonDataFlow : IPersonDataFlow
    {
        private IKafkaService _producer;

        TransformBlock<byte[], Person> entryBlock;

        public PersonDataFlow(IKafkaService producer)
        {
            _producer = producer;

            entryBlock = new TransformBlock<byte[], Person>(data => MessagePackSerializer.Deserialize<Person>(data));

            var enrichBlock = new TransformBlock<Person, Person>(p =>
            {
                p.LastUpdated = DateTime.Now;

                return p;
            });


            var publishBlock = new ActionBlock<Person>(person =>
            {
                Console.WriteLine($"Updated Value:{person.LastUpdated}");
                _producer.SendPersonAsync(person);
            });

            var linkOptions = new DataflowLinkOptions()
            {
                PropagateCompletion = true
            };

            entryBlock.LinkTo(enrichBlock, linkOptions);
            enrichBlock.LinkTo(publishBlock, linkOptions);

        }

        public async Task SendPerson(byte[] data)
        {
            var obj = MessagePackSerializer.Deserialize<Person>(data);
            Console.WriteLine($"Original Date:{obj.LastUpdated}");
            await entryBlock.SendAsync(data);
        }

    }

}
