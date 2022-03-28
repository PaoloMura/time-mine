using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestTimeLordSimulation
{

    [Test]
    public void TestOnePlayerNoTimeTravel()
    {
        // Set some initial values.
        int playerID = 0;
        Vector3 pos = new Vector3(0, 0, 0);
        Quaternion rot = new Quaternion(0, 0, 0, 0);
        Constants.JumpDirection dir = Constants.JumpDirection.Static;

        // Run the simulation.
        TimeLord timeLord = new TimeLord(20);
        timeLord.Connect(playerID, true);
        timeLord.EnterReality(playerID);
        
        for (int i=0; i < 20; i++)
        {
            pos = new Vector3(i, i, i);
            rot = new Quaternion(i, i, i, i);
            PlayerState ps = new PlayerState(0, pos, rot, dir, false);
            timeLord.RecordState(ps);
            timeLord.Tick();
        }

        // Access the final resulting structures from TimeLord.
        Dictionary<int, PlayerState>[] states = timeLord.RevealPlayerStates();
        RealityManager realityManager = timeLord.RevealRealityManager();
        Dictionary<int, List<int>> tailCreations = timeLord.RevealTailCreations();

        // Perform assertions on Player States.
        Assert.AreEqual(20, states.Length, "PlayerStates array does not have the correct length.");

        for (int i=0; i < 20; i++)
        {
            Assert.AreEqual(1, states[i].Count, $"Incorrect number of states stored at frame {i}: {states[i].Count}.");
            Assert.IsTrue(states[i].ContainsKey(0), $"Tail 0 does not have a state stored at frame {i}.");
            
            PlayerState ps = states[i][0];
            Assert.AreEqual(0, ps.PlayerID, $"Player ID not recorded correctly at frame {i}.");
            Assert.AreEqual(0, ps.TailID, $"Tail ID not recorded correctly at frame {i}.");
            Assert.AreEqual(new Vector3(i, i, i), ps.Pos, $"Position not recorded correctly at frame {i}.");
            Assert.AreEqual(new Quaternion(i, i, i, i), ps.Rot, $"Rotation not recorded correctly at frame {i}.");
            Assert.AreEqual(dir, ps.JumpDirection, $"Jump direction not recorded correctly at frame {i}.");
            Assert.IsFalse(ps.Kill, $"Kill not recorded correcty at frame {i}.");
        }

        // Perform assertions on Reality Manager.
        Dictionary<int, FrameData> heads = realityManager.RevealHeads();
        Assert.AreEqual(1, heads.Count, "Incorrect number of realities.");
        Assert.IsTrue(heads.ContainsKey(0), "Reality Manager does not contain a reality for player 0.");
        
        FrameData frameData = heads[0];
        Assert.AreEqual(20, frameData.GetPerceivedFrame(), "Incorrect perceived frame.");
        Assert.AreEqual(0, frameData.GetLastTailID(), "Incorrect last tail ID.");

        List<int> tailFrames = frameData.GetTailFrames();
        Assert.AreEqual(1, tailFrames.Count, "Incorrect number of tail writer pointers.");
        Assert.AreEqual(20, tailFrames[0], "Incorrect tail writer pointer position.");

        // Perform assertions on Tail Creations.
        Assert.AreEqual(1, tailCreations.Count, "Incorrect number of tail creation frames.");
        Assert.IsTrue(tailCreations.ContainsKey(0), "Tail Creations does not contain an entry for frame 0.");
        Assert.AreEqual(1, tailCreations[0].Count, "Incorrect number of tail creations on frame 0.");
        Assert.AreEqual(0, tailCreations[0][0], "Incorrect tail ID created on frame 0.");

        Debug.Log("All assertions pass.");
    }
}
