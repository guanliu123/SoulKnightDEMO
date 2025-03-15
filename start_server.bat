@echo off
REM 设置Python路径为当前目录下的Python312 ，可以替换成自己的Python解释器路径
set PYTHON_PATH=.\Python312\python.exe

REM 启动server目录下的run_server.py
"%PYTHON_PATH%" .\server\run_server.py

pause
