# 基于Orleans做的俄罗斯方块开房间游戏
网络使用websocket

1 首先启动IfCastle.Server，再启动IfCastle.Gateway。此两者分别是Orleans核心逻辑silo的Host，和WebSocket网关。

2 启动IfCastle.Client，连接Gateway进行游戏