using HashidsNet;
using Tuna.Application.Services;

namespace Tuna.Infrastructure;

public class IdGenerator : IIdGenerator
{
    private readonly IHashids _hashids;

    public IdGenerator(IHashids hashids)
    {
        _hashids = hashids;
    }

    public string EncodeHashId(int value)
    {
        return _hashids.Encode(value);
    }

    public string EncodeHashIdLong(long value)
    {
        return _hashids.EncodeLong(value);
    }

    public int DecodeHashId(string value)
    {
        return _hashids.DecodeSingle(value);
    }

    public long DecodeHashIdLong(string value)
    {
        return _hashids.DecodeSingleLong(value);
    }

    public string GenerateUniqueId(int size)
    {
        return Nanoid.Nanoid.Generate(size: size);
    }
}