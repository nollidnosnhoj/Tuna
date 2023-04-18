namespace Tuna.Application.Services;

public interface IIdGenerator
{
    string EncodeHashId(int value);
    string EncodeHashIdLong(long value);
    int DecodeHashId(string value);
    long DecodeHashIdLong(string value);
    string GenerateUniqueId(int size);
}