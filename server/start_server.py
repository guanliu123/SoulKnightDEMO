import os
import sys
import subprocess

def main():
    """启动服务器"""
    # 获取当前脚本所在目录
    script_dir = os.path.dirname(os.path.abspath(__file__))
    
    # 构建run_server.py的路径
    run_server_path = os.path.join(script_dir, "run_server.py")
    
    # 启动服务器
    try:
        subprocess.run([sys.executable, run_server_path], check=True)
    except KeyboardInterrupt:
        print("服务器已停止")
    except subprocess.CalledProcessError as e:
        print(f"服务器启动失败: {e}")

if __name__ == "__main__":
    main()