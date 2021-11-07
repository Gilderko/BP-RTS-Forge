using Forge.Serialization;

namespace Forge.DataStructures
{
	public interface ISignature
	{
		public int GetId();
		void Serialize(BMSByte buffer);
		void Deserialize(BMSByte buffer);
	}
}
