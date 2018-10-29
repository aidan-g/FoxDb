using FoxDb.Interfaces;
using System;
using System.Data;

namespace FoxDb
{
    public class SQLiteTranslation : DatabaseTranslation
    {
        public SQLiteTranslation(IDatabase database)
            : base(database)
        {

        }

        public override DbType GetRemoteType(DbType type)
        {
            switch (type)
            {
                case DbType.Guid:
                    return DbType.String;
            }
            return base.GetRemoteType(type);
        }

        public override object GetLocalValue(DbType type, object value)
        {
            if (value != null && !DBNull.Value.Equals(value))
            {
                switch (type)
                {
                    case DbType.Guid:
                        return Guid.Parse(Converter.ChangeType<string>(value));
                }
            }
            return base.GetLocalValue(type, value);
        }

        public override object GetRemoteValue(DbType type, object value)
        {
            switch (type)
            {
                case DbType.Guid:
                    return Converter.ChangeType<Guid>(value).ToString("d");
            }
            return base.GetRemoteValue(type, value);
        }
    }
}
