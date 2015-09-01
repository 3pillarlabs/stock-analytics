# stock-analytics
Stock Analytic- Signalr based high performance data processing app backed by Redis Cache and High Charts

#Setup
Configure following configuration keys both in *Dashboard (Web Project)* and *StockDataFeeder (Console App)*
- RedisServer: The local redis server to connect with eg localhost:6379
- IsLocal: This should be 1
- SymbolFilePath: Dashboad App_data folder has a symbol file. This is the path to that file

Configure following configuration keys both in Dashboard (Web Project) 
- DataGeneratorProcessPath: Path to the StockDataFeeder executable *StockDataFeeder.exe*
- RedisServerExePath: Path to the Redis Server executable *redis-server.exe*

#Tools and framework
- Install redis-2.8.17

#How to run it
- run redis server on local machine. 
- run the dashboard webapp



