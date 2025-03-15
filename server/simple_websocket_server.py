import asyncio
import websockets
import json

# 定义处理客户端连接的异步函数
async def handle_client(websocket, path):
    try:
        # 使用异步循环接收客户端发送的消息
        async for message in websocket:
            # 打印接收到的原始消息
            print(f"Received raw message: {message}")

            # 尝试将消息解码为JSON格式
            try:
                json_data = json.loads(message)
                print(f"Decoded JSON message: {json_data}")
                # 在这里可以添加对JSON数据的进一步处理逻辑
            except json.JSONDecodeError:
                print("Received message is not valid JSON")

            # 可选：向客户端发送响应
            response = "Message received and processed"
            await websocket.send(response)
    except websockets.exceptions.ConnectionClosedOK:
        print("Client disconnected")

# 定义主函数来启动WebSocket服务器
async def main():
    # 启动WebSocket服务器
    async with websockets.serve(handle_client, "127.0.0.1", 5555):
        print("WebSocket server started on ws://127.0.0.1:5555")
        # 保持服务器运行
        await asyncio.Future()

# 运行主函数
if __name__ == "__main__":
    asyncio.run(main())