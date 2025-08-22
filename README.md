# Connection Pooling Demo

```
SELECT
    c.session_id,
    s.login_name,
    s.host_name,
    c.connect_time,
    s.program_name
FROM sys.dm_exec_connections AS c
JOIN sys.dm_exec_sessions AS s
    ON c.session_id = s.session_id
--WHERE s.program_name LIKE '%ConnectionPoolingDemo%';
```
