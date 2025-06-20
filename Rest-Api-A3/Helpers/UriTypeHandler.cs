using System;
using System.Data;
using Dapper;

namespace Rest_Api_A3.Helpers
{
    // Dapper will now use this whenever it sees a C# property of type Uri
    public class UriTypeHandler : SqlMapper.TypeHandler<Uri>
    {
        public override void SetValue(IDbDataParameter parameter, Uri value)
        {
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value.ToString();
            }
            parameter.DbType = DbType.String;
        }

        public override Uri Parse(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var s = value.ToString().Trim();
            if (string.IsNullOrEmpty(s))
                return null;

            // Allow both absolute (https://…) or simple file names (action_heroes.jpg)
            return new Uri(s, UriKind.RelativeOrAbsolute);
        }
    }
}

