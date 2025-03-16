using System.Collections;
using System.Collections.Generic;
using OPH.Collision.QuadTree;
using UnityEngine;

public class QuadTreeSystem : AbstractSystem
{
    private List<IRect> rects;
    private QTree<IRect> _quadTree;

    protected override void OnInit()
    {
        base.OnInit();
        rects = new List<IRect>();

        InitializeQuadTree();
    }

    private void InitializeQuadTree()
    {
        if (_quadTree == null)
        {
            //var sceneBounds = CalculateSceneBounds();
            _quadTree = QTree<IRect>.CreateRoot(4, 6).InitRect(0, 0, 150,
                150);
        }
    }

    public override void GameUpdate()
    {
        base.GameUpdate();
        TreeUpdate();
    }

    //每帧重构四叉树（效率比每帧更新每个物体高）
    private void TreeUpdate()
    {
        _quadTree.Clear();

        float t1 = Time.realtimeSinceStartup;
        for (int i = 0; i < rects.Count; ++i)
        {
            {
                _quadTree.Insert(rects[i]);
            }
        }

        float t2 = Time.realtimeSinceStartup;
        LogTool.Log("重构四叉树耗时:"+(t2 - t1));
    }

    public void RemoveFromTree(IRect rect)
    {
        rects.Remove(rect);
    }

    public void AddToTree(IRect rect)
    {
        rects.Add(rect);
    }

    public void GetAroundObj(IRect target,List<IRect> list)
    {
        _quadTree.GetAroundObj(target, list);
    }
}
