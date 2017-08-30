using System;

using MessagePack;

namespace NetCoreSample.StandardLibrary
{
    // MessagePack では immutable なオブジェクトも使える (public な setter を要求しない)

    [MessagePackObject]
    public class ImmutablePerson
    {
        [Key("firstName")]
        public string FirstName { get; }

        [Key("lastName")]
        public string LastName { get; }

        [Key("birthDay")]
        public DateTime BirthDay { get; }

        public ImmutablePerson(string firstName, string lastName, DateTime birthDay)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.BirthDay = birthDay;
        }
    }


    public static class PersonArraySerializer
    {
        public static byte[] ToMessagePackBinary(this ImmutablePerson[] that) =>
            MessagePackSerializer.Serialize(that);

        public static byte[] ToLZ4MessagePackBinary(this ImmutablePerson[] that) =>
            LZ4MessagePackSerializer.Serialize(that);

        public static ImmutablePerson[] FromMessagePackBinary(byte[] bytes) =>
            MessagePackSerializer.Deserialize<ImmutablePerson[]>(bytes);

        public static ImmutablePerson[] FromLZ4MessagePackBinary(byte[] bytes) =>
            LZ4MessagePackSerializer.Deserialize<ImmutablePerson[]>(bytes);
    }
}
