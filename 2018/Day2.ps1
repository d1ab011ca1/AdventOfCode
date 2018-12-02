# https://adventofcode.com/2018/day/2
Set-StrictMode -Version Latest

function input {

    # "@($((Get-Clipboard) -join ", "))" | set-clipboard
    # "@('$((Get-Clipboard) -join "', '")')" | set-clipboard

    @('zihrtxagncfpbsnolxydujjmqv', 'zihrtxagwcfpbsoolnydukjyqv', 'aihrtxagwcfpbsnoleybmkjmqv', 'zihrtxagwcfpbsnolgyduajmrv', 'zihrtxgmwcfpbunoleydukjmqv', 'zihqtxagwcfpbsnolesdukomqv', 'zihgtxagwcfpbsnoleydqkjqqv', 'dihrtxagwcqpbsnoleydpkjmqv', 'qihrtvagwcfpbsnollydukjmqv', 'zihrtgagwcfpbknoleyrukjmqv', 'cinrtxagwcfpbsnoleydukjaqv', 'zihrtxagwcfubsneleyvukjmqv', 'zihrtxagwcfpbsvoleydukvmtv', 'zihrtpagwcffbsnolfydukjmqv', 'zihrtxagwcfpbsxoleydtkjyqv', 'zohrvxugwcfpbsnoleydukjmqv', 'zyhrtxagdcfpbsnodeydukjmqv', 'zihrtxaghffpbsnoleyduojmqv', 'oihrtbagwcfpbsnoleyduejmqv', 'zihrtnagwcvpjsnoleydukjmqv', 'iihrtxagwcfpbsnoliyaukjmqv', 'ziartxagwcfpbsnokeydukjmpv', 'eibrtxagwccpbsnoleydukjmqv', 'zihrtxagwczwbsaoleydukjmqv', 'ziiatuagwcfpbsnoleydukjmqv', 'zzhrtxagwckpbsnsleydukjmqv', 'cihrtxaqwcfpbsnoleydkkjmqv', 'zihrtxaywcfpbsnoleydukzdqv', 'zihrtxagwjfpbvnoleydukjmql', 'zihrtxagwcfpbsnoleuduksmql', 'zizrtxxgwcfpbsnoleydukzmqv', 'zihrteagwcfpbsnobeydukjmqe', 'zihrtxafwhfpbsgoleydukjmqv', 'zitrtxagwcfpbsnoleyduvymqv', 'zihrtxauwcfebsnoleygukjmqv', 'zihrtxagwcfpbsnoleydubjrqh', 'zihrtxauwmfpbsnoleydukjmqo', 'zihrtxagwcdpbsnoleydukxmov', 'zihrtmagwcfpbsnoleydukvmlv', 'ziwrtxhgwcfpbsnoleodukjmqv', 'zihytxagacfpbsnoceydukjmqv', 'zihrtxagwcfpbsnolebdugjnqv', 'zihrzxagwcfpbsnjleyduktmqv', 'zihrtxygwcfpbinoleysukjmqv', 'zihrtxagwcfpbmnoveydujjmqv', 'zidrtxagwcfpbsnolexaukjmqv', 'zshrtxagwcepbsnoxeydukjmqv', 'yibrtxagwzfpbsnoleydukjmqv', 'zehrtxagwclpbsnoleymukjmqv', 'zihruxagwcfpbsnoleyhukwmqv', 'zihrwxagwcfpbszolesdukjmqv', 'zihrtpagwcfpbwnoleyuukjmqv', 'ziortxagwcfpssnolewdukjmqv', 'zohrtxagwcfpbwnoleydukjmjv', 'zihrtxagwcfpbsnvleyduzcmqv', 'zihrvxaghcfpbswoleydukjmqv', 'zihrtxagwcfpssnolwydukzmqv', 'zjhrttagwcfpbsnolfydukjmqv', 'zihrtxagwjfpbsnoljydukpmqv', 'ziwrtxagwczpbsnoljydukjmqv', 'zinrtxagwcfpbvfoleydukjmqv', 'zihrgragwcfpbsnoleydutjmqv', 'zihrtxagwcfpbsnozeydukffqv', 'zihrtxagwcfpbsmoleydxkumqv', 'rihwtxagwcfpbsxoleydukjmqv', 'ziqrtxagwcfpbsnqlevdukjmqv', 'zihrtxagwchpbsnoleydufamqv', 'sihrtxagwcfpbsnoleldukjmqp', 'zihrtxagwcrpbsnoleydvojmqv', 'zihrtxacwcfpbsnoweyxukjmqv', 'zihrtxagwcfpbsnolajmukjmqv', 'zzfrtxagwcfpbsnoleydukjmvv', 'zixrtxagwcfpbqnoleydukjgqv', 'zihitxaqwcfpbsnoleadukjmqv', 'zilrtxagecfxbsnoleydukjmqv', 'zihrtxagwcfpbypoleycukjmqv', 'zidrtxagdtfpbsnoleydukjmqv', 'lehrtxagxcfpbsnoleydukjmqv', 'zihrlxagwcfpbsncneydukjmqv', 'zihroxagbcspbsnoleydukjmqv', 'zihrtxagwcfkzsnolemdukjmqv', 'zihrtxagwcfpbsqeleydukkmqv', 'zihrjxagwcfpesnolxydukjmqv', 'zifrtxagwcfpbsooleydukkmqv', 'zirwtxagwcfpbsnoleydukzmqv', 'zjhntxagwcfpbsnoleydunjmqv', 'ziorexagwcfpbsnoyeydukjmqv', 'zhhrtlagwcfybsnoleydukjmqv', 'zirrtxagwvfsbsnoleydukjmqv', 'bihrtxagwofpbsnoleadukjmqv', 'dihrtxagwcfpksnoleydukjlqv', 'zihrrxagecfpbsnoleydukjmyv', 'zijrtxagwmfpbsnoleyduljmqv', 'zihrtxagwcfpbsnolecdukjpqs', 'zchrtxagwcfpbsnolehdukjmwv', 'rmhrtxagwcfpbsnoleydkkjmqv', 'zohrotagwcfpbsnoleydukjmqv', 'zihwtxagsifpbsnwleydukjmqv', 'zihrtxagicfpbsnoleydukjxqn', 'zihrtxsgwcfpbsntleydumjmqv', 'zihrlxagzgfpbsnoleydukjmqv', 'aihjtxagwdfpbsnoleydukjmqv', 'zifrtxagwcfhbsnoleddukjmqv', 'zihrtyagwcfpbsooleydtkjmqv', 'zihrtxxgwcfpbsnolerhukjmqv', 'zihqtxalwcfppsnoleydukjmqv', 'zfkrvxagwcfpbsnoleydukjmqv', 'zihptxagwcfpbseoleydukjmdv', 'zihrtxagwcfpeonoleyiukjmqv', 'nidrtxagwcfpbsnoleyhukjmqv', 'zihrtxagwcfjbsnolsydukjmqg', 'zghryxagwcfgbsnoleydukjmqv', 'zihwtxagwcfpbsnoleydugjfqv', 'zihryxagwjfpbsnoleydujjmqv', 'zihrtxagwcfpbsnolekdukymql', 'zfhrtxaownfpbsnoleydukjmqv', 'zamrtxagwcfpbsnoleyduzjmqv', 'ibhrtxagwcfpbsnoleydukjmfv', 'zihrtxagwcfpssnoseydukjmuv', 'zihrtxagwcfpbsnoljydukjhqs', 'zihrtxagwqfmbsnoleidukjmqv', 'zfdrtxagwchpbsnoleydukjmqv', 'iihrtxagqcfpbsnoleydukjmqn', 'mihrtxagwcfpbsqoleydukjbqv', 'zihttxagwcfpbsnoleyduljmqk', 'zzhrtxagwcfpzseoleydukjmqv', 'zdhrtxagbcfpbsnoleyduyjmqv', 'zihxtxagwcfpbsnolwrdukjmqv', 'zghrtxagwcypbynoleydukjmqv', 'zihrtxaiwcfppsnoleydukgmqv', 'zitatxagwcfobsnoleydukjmqv', 'znhrtxagwcfpysnoleydukjqqv', 'zihrtxagwcfppsnoleoyukjmqv', 'ziorgxagwcfpbsnolekdukjmqv', 'zihrtxagwcfpbfnoleydwkjpqv', 'zihrtxnrwcfpbsnolnydukjmqv', 'rihrtxagwcfpbsnolepdjkjmqv', 'zihrtxagwcfzbsnoceydukjmkv', 'zihrtxagwcfpysnoaeidukjmqv', 'zihrmxagwcfpbsnoleydukjmuq', 'gihrtxagwcvpbsnoleydukcmqv', 'zihrtxagocfpbsnoleydukqmnv', 'zihrtxagwcfpesnoleyluklmqv', 'zghrtxagwcfzbsnoleydukjmgv', 'zihrtxugqqfpbsnoleydukjmqv', 'zirrtcagwcfpbsnoleydfkjmqv', 'zihitxagwcfpjsnoleydnkjmqv', 'zihrtxqgwcfpbsnsleydukjmqy', 'iihrtxagwyfpbsnoleydukjmqu', 'zihrsxagwcfpbsnsleydukzmqv', 'zihrtxawwcfpbsnoleydzkjmuv', 'dihrkxagwcfpbsfoleydukjmqv', 'zihrtxaqwcfpbvnoleydukjmqt', 'zihntxdgwcfpbsnogeydukjmqv', 'zihrtxagwcdpxsnolxydukjmqv', 'zihrtxagwcfpbsaoleydunjaqv', 'zihrtyagwcfpbsnoleyduqjmqt', 'zihrtxagwtfpbsnoleoyukjmqv', 'zihrjiagwcfpbsnobeydukjmqv', 'zihrtxqgwcfpbsnoleydykdmqv', 'zihrhxmgwcfpbsnmleydukjmqv', 'zihatxlgwcfpbsnoleydukpmqv', 'zihrtxcgwcspbsnoleypukjmqv', 'zihrtkagqcfpbsaoleydukjmqv', 'ziqrtxagwcfabsnoleydukrmqv', 'zihwtxagwifpbsnwleydukjmqv', 'zitrtnagwcfpbsnoleddukjmqv', 'wihrtxagwcfpbsioyeydukjmqv', 'zihrtxagwclpystoleydukjmqv', 'zihmtxagwcfpbsnolfydukjmlv', 'zihrtxagechpbsnoleydutjmqv', 'zihrtxagwcfebsnolnydukjmuv', 'zihrtxagncmpbsnoleydukjmqs', 'zihrvxagocfpbsnoleydukcmqv', 'zihrtxagwcjcbsnolejdukjmqv', 'wihrtxagwcfpbogoleydukjmqv', 'kivrtxagwcfpgsnoleydukjmqv', 'zihrtxagwafpbhnoleydukjcqv', 'zihrtwagtcfpbsnolxydukjmqv', 'vihrtxagwcfpbsneletdukjmqv', 'zihlnxagwcfpbsnoleydukjmqb', 'zihrtxagwcfpbsnoleydukjuuc', 'zihrtxagwcfpbwntleadukjmqv', 'fihrtxagwcfpbsnoleydvkjmqw', 'zihrtxaowcfpbunoleyduljmqv', 'zthrtxagwcfpbtnoleydukomqv', 'xihltxagwcfpbsnoleydukjrqv', 'ziyrnxagwcfpbsnoleydukjmhv', 'zihrtxazwcfpbsnileyduejmqv', 'zihrtxagwcfibsnoliydukjmsv', 'zihrtxggwcfpbsnoleydugjmqj', 'zrartxagwcffbsnoleydukjmqv', 'zidrtxaqwcfpbsnoleyduksmqv', 'zirrtxagwcypbsnoleydtkjmqv', 'rihrtxagwcrpbsnoheydukjmqv', 'zihrtxagwcfpbsnoleydpkjmzs', 'zihrtxagbcfpbsnodbydukjmqv', 'fihrtxaqwcfpbsnolaydukjmqv', 'vihrtxbgwcfpbsnolemdukjmqv', 'zihrtxapwcfubsnoleydukmmqv', 'zihrtxagwcfpbgnolfydunjmqv', 'zihrtxagwcypbsnokeyduvjmqv', 'zihntxagwcfpbsnoieydukbmqv', 'zihbtxagwkfpbsnolpydukjmqv', 'zihrtxagwcfibsnoleydikjmqb', 'jihrtxvgwcfpbsnoleydukjmqp', 'zihrtxagwcfpbjnqleydukjmlv', 'zibrtxagwcfpbzvoleydukjmqv', 'zihrtxagwafgbsnbleydukjmqv', 'zihjctagwcfpbsnoleydukjmqv', 'zahrtxagwcepbsnoleddukjmqv', 'zihetxagwcfpbsnoleydumjmsv', 'zihrtvagwcfpbbnoleydukdmqv', 'zbhrxxagwkfpbsnoleydukjmqv', 'jfhrtxagwcftbsnoleydukjmqv', 'yihrtxagwcfvbsnoleyduksmqv', 'ziartxaewcfpbsnoleyduhjmqv', 'zihrtxagwcfpbsnoozyduzjmqv', 'cihotxagwcfpysnoleydukjmqv', 'zihrtxagwcfpusnolwydxkjmqv', 'zihrtxagwcfpbsnoleedmgjmqv', 'zihrtxaghcfpmsnoleydukqmqv', 'ziortxagwcfpbsboleidukjmqv', 'zihrtxagwcfybsnoleyqxkjmqv', 'zihrtxamwcfpbsngleydukjmqx', 'zihrtxagwcfpbsnoleyduusmqu', 'zihftxagwcfpssnwleydukjmqv', 'zihrtxagwcfkbsnomeydukjmsv', 'zihrtxagwcvpbsnooeydwkjmqv', 'zihrtxagwcfpbsnoleycekumqv', 'jahrtxagwcfpbsnoleydukjmmv', 'zihrtxabwcfpbsnzheydukjmqv', 'zihrtxagwctpbsnoleydwkjmhv', 'zihrtpagwcfpbsnoleydzkjmqh', 'zihwtxagwcfpbsnollydukjrqv', 'zihrtxagwcfpusnoleydsvjmqv', 'zibrtxagwcfpasnoleydukjmbv', 'zchrtmagwcfpbsnoleydukjmwv', 'ziertxbgwyfpbsnoleydukjmqv', 'zitrtxagwcfpbhnoweydukjmqv', 'zisrtxkgwcfpbsnopeydukjmqv', 'zihrtxcgwdfpbynoleydukjmqv', 'iihrtxajwcvpbsnoleydukjmqv', 'zihuwxapwcfpbsnoleydukjmqv', 'zihrtxngwcfqbsnoleyiukjmqv', 'ziqrtxagjcfpbsnoleydukjmqi', 'zifrtxarwctpbsnoleydukjmqv', 'zihxgxagwcfpbpnoleydukjmqv', 'giprtxagwcdpbsnoleydukjmqv', 'zihrtxagwmfpbsnodeydukjbqv')
}

function part1 {
    $2 = 0
    $3 = 0
    foreach($i in input) {
        $h=@{}
        foreach($c in $i.GetEnumerator()) {$h[$c] += 1}
        if ($h.Values.Where{$_ -eq 2}){ $2++ }
        if ($h.Values.Where{$_ -eq 3}){ $3++ }
    }

    $2 * $3
}

function part2 {
    $in = input
    for ($i = 0; $i -lt $in.Count; $i++) {
        $s1 = $in[$i]
        for ($j = $i+1; $j -lt $in.Count; $j++) {
            $s2 = $in[$j]

            $diff = 0
            $diffAt = -1
            foreach($n in (0..25)) {
                if ($s1[$n] -ne $s2[$n]) {
                    if ((++$diff) -gt 1) {break}
                    $diffAt = $n
                }
            }
            if ($diff -eq 1) { return $s1.Remove($diffAt, 1) }
        }
    }
}

Clear-Host
#part1 #= 8892
part2 #= 'zihwtxagifpbsnwleydukjmqv'
