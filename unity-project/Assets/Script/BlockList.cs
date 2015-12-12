using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BlockList : IEnumerable
{
    private List<List<Block>> BlockListEnmuerable;

    public BlockList()
    {
        BlockListEnmuerable = new List<List<Block>>();
        initMap();
    }

    public IEnumerator GetEnumerator()
    {
        foreach (List<Block> blockRow in BlockListEnmuerable)
        {
            foreach(Block block in blockRow)
            {
                yield return block;                
            }
        }
    }

    public IEnumerable GetEnumeratorRoom()
    {
        foreach (List<Block> blockRow in BlockListEnmuerable)
        {
            foreach (Block block in blockRow)
            {
                if(block.GetBlockType() == Block.BlockType.Room)
                {
                    yield return block;
                }
                
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    private void initMap()
    {
        for (int y = 0; y < Map.VERTIVAL; y++)
        {
            BlockListEnmuerable.Add(initAreaListElement(y));
        }
    }

    private List<Block> initAreaListElement(int y)
    {
        List<Block> areaListElement = new List<Block>();

        for (int x = 0; x < Map.HORIZONTAL; x++)
        {
            Block area = new Block(x,y);            
            area = area.getBlockObject().GetComponent<Block>();
            area.InitBlock(x, y);
            areaListElement.Add(area);
        }

        return areaListElement;

    }

    public Block GetAreaGroup(int x, int y)
    {
        // X座標チェック
        if (x < 0 || x >= Map.HORIZONTAL)
        {
            return null;
        }

        if (y < 0 || y >= Map.VERTIVAL)
        {
            return null;
        }

        return BlockListEnmuerable[y][x];
    }
}