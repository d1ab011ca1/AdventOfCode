<Query Kind="Program" />

#LINQPad optimize+
//const string data = "389125467"; // example
const string data = "942387615";
void Main()
{	
	//Part1();
	Part2();
}

void Part1()
{
	var cups = data.ToCharArray()
		.Select(c => (int)c - (int)'0')
		.ToArray();
	//cups.Dump();

	Play(cups, 100).Cast<object>().Count();
	$"cups:  { String.Join("  ", cups) }".Dump($"-- final --");

	var oneIdx = Array.IndexOf(cups, 1);
	var answer = String.Join(null, cups.Skip(oneIdx + 1).Concat(cups.Take(oneIdx)));
	answer.Dump("Part 1"); // 36542897
}

void Part2()
{
	var cups = data.ToCharArray()
		.Select(c => (int)c - (int)'0')
		.Concat(Enumerable.Range(data.Length + 1, 1000000 - data.Length))
		.ToArray();

	Play(cups, 10 * cups.Length).Cast<object>().Count();//.Dump();

	var oneIdx = Array.IndexOf(cups, 1);
	long next1 = cups[(oneIdx + 1) % cups.Length];
	long next2 = cups[(oneIdx + 2) % cups.Length];
	$"{next1} * {next2} = {next1 * next2}".Dump("Part 2"); // 562136730660
}

IEnumerable Play(int[] cups, int numMoves)
{
	int length = cups.Length;
	$"Playing {numMoves} moves with {length} cups".Dump();

	var buffer = new int[length];
	int copyIntoBuffer(int bufferIdx, int[] src, int srcIdx, int count)
	{
		if (count == 0)
			return bufferIdx;
			
		if (bufferIdx + count > length)
		{
			int n = length - bufferIdx;
			copyIntoBuffer(bufferIdx, src, srcIdx, n);
			srcIdx = (srcIdx + n) % src.Length;
			return copyIntoBuffer(0, src, srcIdx, count - n);
		}

		if (srcIdx + count > src.Length)
		{
			int n = src.Length - srcIdx;
			Array.Copy(src, srcIdx, buffer, bufferIdx, n);
			Array.Copy(src, 0, buffer, bufferIdx + n, count - n);
		}
		else
		{
			Array.Copy(src, srcIdx, buffer, bufferIdx, count);
		}
		return (bufferIdx + count) % length;
	};

	var curIdx = 0;
	var pickedUp = new int[3];
	var timer = System.Diagnostics.Stopwatch.StartNew();
	var reportPeriod = TimeSpan.FromMinutes(5);
	for (int move = 1; move <= numMoves; move++)
	{
		if (timer.Elapsed >= reportPeriod)
		{
			$"Move {move}, {100.0 * move / numMoves:.00}%".Dump();
			timer.Restart();
		}
		
		// The crab picks up the three cups that are immediately clockwise of the 
		// current cup. They are removed from the circle; cup spacing is adjusted 
		// as necessary to maintain the circle.
		//pickedUp = Enumerable.Range(curIdx + 1, 3).Select(i => cups[i % length]).ToArray();
		pickedUp[0] = cups[(curIdx + 1) % length];
		pickedUp[1] = cups[(curIdx + 2) % length];
		pickedUp[2] = cups[(curIdx + 3) % length];

		// The crab selects a destination cup: the cup with a label equal to the 
		// current cup's label minus one. If this would select one of the cups that 
		// was just picked up, the crab will keep subtracting one until it finds a 
		// cup that wasn't just picked up. If at any point in this process the value 
		// goes below the lowest value on any cup's label, it wraps around to the 
		// highest value on any cup's label instead.
		var curLbl = cups[curIdx];
		var destLbl = curLbl;
		do { 
			// !!! Remember ids are 1-based !!!
			destLbl = destLbl == 1 ? length : destLbl - 1;
		} while (pickedUp[0] == destLbl || pickedUp[1] == destLbl || pickedUp[2] == destLbl);

		//var s1 = String.Join(' ', cups.Take(100).Select(c => c == curLbl ? $"({c})" : $" {c}"));
		//$"cups: {s1}\npick up: { String.Join(' ', pickedUp) }\ndestination: { destLbl }".Dump($"-- move {move} --");

		// The crab places the cups it just picked up so that they are immediately 
		// clockwise of the destination cup. They keep the same order as when they 
		// were picked up.
		if (destLbl != curLbl)
		{
			// move the 3 picked up cups to destIdx+1
			var destIdx = Array.IndexOf(cups, destLbl);
			if (curIdx < destIdx)
			{
				var bufIdx = curIdx + 1;
				bufIdx = copyIntoBuffer(bufIdx, cups, curIdx + 4, destIdx - (curIdx + 4) + 1);
				bufIdx = copyIntoBuffer(bufIdx, pickedUp, 0, 3);
				bufIdx = copyIntoBuffer(bufIdx, cups, (destIdx + 1) % length, (curIdx + length) - destIdx);
			}
			else
			{
				var bufIdx = (curIdx + 1) % length;
				bufIdx = copyIntoBuffer(bufIdx, cups, (curIdx + 4) % length, (destIdx + length) - (curIdx + 4) + 1);
				bufIdx = copyIntoBuffer(bufIdx, pickedUp, 0, 3);
				bufIdx = copyIntoBuffer(bufIdx, cups, destIdx + 1, curIdx - destIdx);
			}
			
			// swap arrays
			var temp = cups;
			cups = buffer;
			buffer = temp;
		}

		// The crab selects a new current cup: the cup which is immediately clockwise 
		// of the current cup.
		curIdx = (curIdx + 1) % length;
	}
	
	yield return 1;
}

public static string Helper()
{
	return "";
}

// You can define other methods, fields, classes and namespaces here
