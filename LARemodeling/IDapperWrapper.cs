﻿using System.Data;

namespace LARemodeling;

public interface IDapperWrapper
{
    int Execute(IDbConnection? conn, string? sql, object? param = null, IDbTransaction? transaction = null,
        int? commandTimeout = null, CommandType? commandType = null);

    IEnumerable<T> Query<T>(IDbConnection? conn, string? sql, object? param = null, IDbTransaction? transaction = null,
        bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
}
