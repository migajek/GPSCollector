using System.Linq;
using NUnit.Framework;
using Serialization;

namespace Tests
{
    class TestPacket
    {
        [SerializeField(0)]
        public int Data { get; set; }
        [SerializeField(1)]
        public byte Byte { get; set; }
    }

    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void SerializedDataSize()
        {
            var packet = new TestPacket() {Byte = 123, Data = 54321};
            var bf = new BinarySerializer();
            var bytes = bf.Serialize(packet).ToArray();
            Assert.AreEqual(5, bytes.Length);
        }
    }
}