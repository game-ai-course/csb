using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {
        Console.ReadLine();
        var count = int.Parse(Console.ReadLine());
        var cx = new List<int>();
        var cy = new List<int>();
        for(int i=0; i<count; i++) 
		{
            var inputs = Console.ReadLine().Split();
            cx.Add(int.Parse(inputs[0]));
            cy.Add(int.Parse(inputs[1]));
        }
        while (true)
        {
            for(int pindex=0; pindex<2; pindex++)
			{
                var inputs = Console.ReadLine().Split(' ');
                var x = int.Parse(inputs[0]);
                var y = int.Parse(inputs[1]);
                var vx = int.Parse(inputs[2]);
                var vy = int.Parse(inputs[3]);
                var nextCheckpointId = int.Parse(inputs[5]);
                var nx = cx[nextCheckpointId];
                var ny = cy[nextCheckpointId];
                var tx = nx- 3*vx;
                var ty = ny- 3*vy;
                Console.WriteLine(tx + " " + ty + " 100");
            }
            Console.ReadLine();
            Console.ReadLine();
        }
    }
}
