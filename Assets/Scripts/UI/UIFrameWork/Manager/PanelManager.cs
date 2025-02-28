using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIFrameWork
{
    //管理所有同时出现的面板对象，采用堆栈方式
    public class PanelManager : SingletonBase<PanelManager>
    {
        private Stack<BasePanel> panelStack = new Stack<BasePanel>();
        //键是UIInfo
        private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
        
        //添加一个面板到顶部
        // public void Push(BasePanel nextPanel)
        // {
        //     if (nextPanel == null)
        //         return;
        //     if (stackPanel.Count > 0)
        //     {
        //         //添加新面板时原顶部面板要停止
        //         BasePanel topPanel = stackPanel.Peek();
        //         topPanel.OnPause();
        //     }
        //     stackPanel.Push(nextPanel);
        //     panels.TryAdd(nextPanel, 1);
        //     GameObject panel = UIManager.Instance.GetSingleUI(nextPanel.UIType);
        //     nextPanel.OnEnter();//新面板要调用进入方法
        // }
        
        // public void OpenPanel(BasePanel nextPanel)
        // {
        //     if (nextPanel == null)
        //         return;
        //
        //     //if(topPanel!=null) topPanel.OnPause();
        //     //加入失败说明面板已经打开了不要重复打开
        //     if (!panelDic.TryAdd(nextPanel.UIType.Path, nextPanel))
        //     {
        //         LogTool.Log("面板"+nextPanel.UIType.Name+"已打开，请勿重复打开！");
        //         return;
        //     }
        //     //GameObject panel = UIManager.Instance.GetSingleUI(nextPanel.UIType);
        //     nextPanel.OnEnter();//新面板要调用进入方法
        // }
        //
        // // public void Pop()//弹出顶部面板
        // // {
        // //     if (stackPanel.Count > 0)
        // //         stackPanel.Pop().OnExit();//弹出的面板要销毁
        // //     if (stackPanel.Count > 0)
        // //         stackPanel.Peek().OnResume();//新的顶部面板要恢复
        // // }
        //
        // // public void Clear()//清空所有面板
        // // {
        // //     while (stackPanel.Count > 0)//每一个面板都要销毁
        // //         stackPanel.Pop().OnExit();
        // // }
        //
        // public void CloseAllPanel()
        // {
        //     foreach (var item in panelDic)
        //     {
        //         item.Value.OnExit();
        //     }
        //     
        //     panelDic.Clear();
        // }
        //
        // public void ClosePanel(string panelInfo)
        // {
        //     if (panelDic.ContainsKey(panelInfo))
        //     {
        //         panelDic[panelInfo].OnExit();
        //         panelDic.Remove(panelInfo);
        //     }
        // }
    
        // 打开面板
        public void OpenPanel(BasePanel nextPanel)
        {
            if (nextPanel == null)
            {
                return;
            }
            
            if (!panelDic.TryAdd(nextPanel.UIType.Path, nextPanel))
            {
                LogTool.Log("面板"+nextPanel.UIType.Name+"已打开，请勿重复打开！");
                return;
            }
            // 暂停当前顶层面板
            while (panelStack.Count > 0)
            {
                var t = panelStack.Peek();
                if (!t.IsClose)
                {
                    t.OnPause();
                    break;
                }
                panelStack.Pop();
            }
            panelStack.Push(nextPanel);
            nextPanel.OnEnter();//新面板要调用进入方法
        }

        // 关闭顶层面板
        public void PopPanel()
        {
            if(panelStack.Count > 0)
            {
                BasePanel topPanel = panelStack.Pop();
                topPanel.OnExit();
            
                // 恢复新顶层面板
                if(panelStack.Count > 0)
                    panelStack.Peek().OnResume();
            }
        }

        // 强制关闭任意面板
        public void ClosePanel(string panelKey)
        {
            if(panelDic.TryGetValue(panelKey, out BasePanel panel))
            {
                // 从栈中移除（需要自定义栈的遍历方法）
                //panelStack.Remove(panel);
                panel.OnExit();
            
                // 自动恢复正确的顶层面板
                while (panelStack.Count > 0)
                {
                    var t = panelStack.Peek();
                    if (!t.IsClose)
                    {
                        t.OnResume();
                        break;
                    }
                    panelStack.Pop();
                }
            }
        }
        
        //清空所有面板
        public void CloseAllPanel()
        {
            //每一个面板都要销毁
            while (panelStack.Count > 0)
            {
                var t=panelStack.Pop();
                if(!t.IsClose) t.OnExit();
            }
        }
    }
}

