<Query Kind="Statements" />

const ulong cardPkey = 14222596; //real
const ulong doorPkey = 4057428; //real
//const ulong cardPkey = 5764801; //example
//const ulong doorPkey = 17807724; //example

var cardLoopSize = 0;
for (var k = 1UL; k != cardPkey; k = (k * 7) % 20201227)
	++cardLoopSize;
cardLoopSize.Dump("cardLoopSize");

var doorLoopSize = 0;
for (var k = 1UL; k != doorPkey; k = (k * 7) % 20201227)
	++doorLoopSize;
doorLoopSize.Dump("doorLoopSize");

//var key = 1UL;
//for (var loop = 0; loop < doorLoopSize; ++loop)
//	key = (key * 7) % 20201227;
//(doorPkey == key).Dump("got doorPKey?");
//
//key = 1UL;
//for (var loop = 0; loop < cardLoopSize; ++loop)
//	key = (key * 7) % 20201227;
//(cardPkey == key).Dump("got cardPKey?");

var key = 1UL;
for (var loop = 0; loop < doorLoopSize; ++loop)
	key = (key * cardPkey) % 20201227;
key.Dump("encryption key");

key = 1UL;
for (var loop = 0; loop < cardLoopSize; ++loop)
	key = (key * doorPkey) % 20201227;
key.Dump("encryption key");
