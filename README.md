# tcp-app-demo

## Run the demo

1. Run the server

```sh
# Run the command in a new terminal
dotnet run --project tcp-server
```

2. Run the client

```sh
# Run the command in a new terminal
dotnet run --project tcp-client
```

3. The protocol of demo

station-01
```
C: #station-01/data-01
S: ok
C: #station-01/data-02
S: ok
...
C: #station-01/data-25
S: ok
```

station-02
```
C: #station-02/data-01
S: ok
C: #station-02/data-02
S: ok
...
C: #station-02/data-25
S: ok
```
...

...

station-99
```
C: #station-99/data-01
S: ok
C: #station-99/data-02
S: ok
...
C: #station-99/data-25
S: ok
```

4. Notice:

Don't print stream(network stream) to console/terminal in production mode.

The console stream is slow and can block the source stream (ex, network stream) cause performance problems.
