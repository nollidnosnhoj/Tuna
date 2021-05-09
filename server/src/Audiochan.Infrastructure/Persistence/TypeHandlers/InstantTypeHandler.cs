using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using NodaTime;

namespace Audiochan.Infrastructure.Persistence.TypeHandlers
{
    public class InstantTypeHandler : SqlMapper.TypeHandler<Instant>
    {
        public static readonly InstantTypeHandler Default = new InstantTypeHandler();
        
        public override void SetValue(IDbDataParameter parameter, Instant value)
        {
            parameter.Value = value.ToDateTimeUtc();

            if (parameter is SqlParameter sqlParameter)
            {
                sqlParameter.SqlDbType = SqlDbType.DateTime2;
            }
        }

        public override Instant Parse(object value)
        {
            if (value is DateTime dateTime)
            {
                var dt = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                return Instant.FromDateTimeUtc(dt);
            }

            if (value is DateTimeOffset dateTimeOffset)
            {
                return Instant.FromDateTimeOffset(dateTimeOffset);
            }

            throw new DataException("Cannot convert " + value.GetType() + " to NodaTime.Instant");
        }
    }
}