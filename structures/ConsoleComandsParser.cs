using DemoCleaner3.ExtClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoCleaner3.structures {
    //Data info what can be obtained from the time string
    public class TimeStringInfo {
        public string source;               //source string
        public TimeSpan time;               //time
        public string oName;                //online username
        public string lName;                //online q3df login
    }

    public class DateStringInfo {
        public string source;
        public DateTime? recordDate;
    }

    public class AdditionalTimeInfo {
        public string source;               //source string

        public TimeSpan time = TimeSpan.Zero;

        public List<TimeSpan> cpData = new List<TimeSpan>();    //checkpoints

        public int offset = -1;
        public int pmove_depends = -1;
        public int pmove_fixed = -1;
        public int sv_fps = -1;
        public int com_maxfps = -1;
        public int g_sync = -1;
        public int pmove_msec = -1;
        public int all_weapons = -1;
        public int no_damage = -1;
        public int enable_powerups = -1;

        public Dictionary<string, string> toDictionary() {
            var lines = new Dictionary<string, string>();

            if (pmove_fixed >= 0) { lines.Add("pmove_fixed", pmove_fixed.ToString()); }
            if (sv_fps >= 0) { lines.Add("sv_fps", sv_fps.ToString()); }
            if (com_maxfps >= 0) { lines.Add("com_maxfps", com_maxfps.ToString()); }
            if (g_sync >= 0) { lines.Add("g_sync", g_sync.ToString()); }
            if (pmove_msec >= 0) { lines.Add("pmove_msec", pmove_msec.ToString()); }

            if (all_weapons >= 0) { lines.Add("all_weapons", all_weapons.ToString()); }
            if (no_damage >= 0) { lines.Add("no_damage", no_damage.ToString()); }
            if (enable_powerups >= 0) { lines.Add("enable_powerups", enable_powerups.ToString()); }

            return lines;
        }

        public bool isTr = false;
    }

    public class ConsoleComandsParser {

        public List<TimeStringInfo> timeStrings = new List<TimeStringInfo>();
        public List<DateStringInfo> dateStrings = new List<DateStringInfo>();
        public List<AdditionalTimeInfo> additionalInfos = new List<AdditionalTimeInfo>();


        public ConsoleComandsParser(Dictionary<long, string> consoleCommands) {
            var timerStartedCount = 0;
            foreach (var kv in consoleCommands) {
                var value = kv.Value;

                //print "Date: 10-25-14 02:43\n"
                if (value.StartsWith("print \"Date:")) {
                    dateStrings.Add(new DateStringInfo() {
                        source = value,
                        recordDate = ConsoleStringUtils.getDateForDemo(value)
                    });
                } else if (value.Contains("reached the finish line in")) {
                    //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
                    timeStrings.Add(new TimeStringInfo() {
                        source = value,
                        time = ConsoleStringUtils.getTimeOnline(value),
                        oName = ConsoleStringUtils.getNameOnline(value)
                    });
                } else if (value.Contains("broke the server record") || value.Contains("you are now rank") || value.Contains("equalled the server record with")) {
                    //chat "^7^3blastoise^3(^7^4DF^7/^4/^7/ Mr. 10003^3)^2 broke the server record with ^348:496 ^0[^2-1:496^0] ^2!!!^7"
                    //chat "^7^1E^3nter^3(^7^1E^3nter^3)^2, you are now rank ^32 ^2of ^35 ^2with ^317:896 ^0[^1+0:032^0]^7"
                    //chat "^7B^21^7ade^3(^7B^21^7ade^3)^2 equalled the server record with ^30:008^2!!!^7"
                    var result = ConsoleStringUtils.getNameQ3df(value);
                    if (result.HasValue) {
                        timeStrings.Add(new TimeStringInfo() {
                            source = value,
                            time = result.Value.time,
                            oName = result.Value.name,
                            lName = result.Value.q3dfName
                        });
                    }
                } else if (value.StartsWith("print \"Time performed by")) {
                    //defrag 1.9+
                    //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"
                    timeStrings.Add(new TimeStringInfo() {          
                        source = value,
                        time = ConsoleStringUtils.getTimeOfflineNormal(value),
                        oName = ConsoleStringUtils.getNameOffline(value)
                    });
                } else if (value.StartsWith("NewTime")) {
                    //defrag 1.80
                    //"NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg"
                    //pro-q3dm6[df.cpm]00.07.200(Viper.uk).dm_68
                    timeStrings.Add(new TimeStringInfo() {
                        source = value,
                        time = ConsoleStringUtils.getTimeOld1(value),
                        oName = ConsoleStringUtils.getNameOfflineOld1(value)
                    });
                } else if (value.StartsWith("print \"^3Time Performed:")) {
                    //defrag 1.5
                    //"print \"^3Time Performed: 25:912 (defrag 1.5)\n^7\""
                    //acc_homer[df.vq3]00.25.912(mcuni7_7.russia).dm_67 
                    timeStrings.Add(new TimeStringInfo() {         
                        source = value,
                        time = ConsoleStringUtils.getTimeOfflineNormal(value)
                    });
                } else if (value.StartsWith("newTime")) {
                    //defrag 1.43
                    //newTime 47080
                    //erde1[df.vq3]00.47.080(#bon3Sektor.germany).dm_68
                    timeStrings.Add(new TimeStringInfo() {
                        source = value,
                        time = ConsoleStringUtils.getTimeOld3(value)
                    });
                } else if (value.StartsWith("TimerStarted")) {
                    //for checking tr
                    timerStartedCount++;
                } else if (value.StartsWith("TimerStopped")) {
                    //defrag 1.80
                    //TimerStopped 7600 2 1464 4016 Stats 8 1 120 125 0 8 1 1 0  
                    //bra_run3[df.vq3]00.07.600(VipeR.russia).dm_68
                    var info = ConsoleStringUtils.parseAdditionalInfo(value);
                    if (timerStartedCount > 1) {
                        info.isTr = true;
                    }
                    timerStartedCount = 0;
                    additionalInfos.Add(info);
                }
            }
        }

        //get fastest time strings for specified player by his name
        public TimeStringInfo getFastestTimeStringInfo(DemoNames names) {
            TimeStringInfo fastestTimeString = null;
            if (timeStrings.Count == 0 && additionalInfos.Count > 0) {
                var minAdditional = Ext.MinOf(additionalInfos, x => (long)x.time.TotalMilliseconds);
                new TimeStringInfo() {
                    time = minAdditional.time,
                    source = minAdditional.source
                };
            } else if (timeStrings.Count == 1) {
                fastestTimeString = timeStrings.First();
            } else if (timeStrings.Count >= 1) {
                var cuStrings = timeStrings.Where(x => (!string.IsNullOrEmpty(x.oName) && (x.oName == names.dfName || x.oName == names.uName)));
                if (cuStrings.Count() == 0) {
                    var groups = timeStrings.GroupBy(x => x.oName);
                    if (groups.Count() == 1) {
                        cuStrings = timeStrings;
                    }
                }
                if (cuStrings.Count() > 0) {
                    fastestTimeString = Ext.MinOf(cuStrings, x => (long)x.time.TotalMilliseconds);
                    var fastestList = cuStrings.Where(x => x.time == fastestTimeString.time);
                    if (fastestList.Count() > 1) {
                        var fastWithLogin = fastestList.FirstOrDefault(x => x.lName != null);
                        if (fastWithLogin != null) {
                            return fastWithLogin;
                        } else {
                            return fastestTimeString;
                        }
                    } else {
                        return fastestTimeString;
                    }
                }
            }
            return fastestTimeString;
        }

        public TimeStringInfo getGoodTimeStringInfo(DemoNames names, long time) {
            if (time > 0) {
                for (int i = 0; i < timeStrings.Count; i++) {
                    if (!string.IsNullOrEmpty(timeStrings[i].oName)) {
                        var sameName = (timeStrings[i].oName == names.uName || timeStrings[i].oName == names.dfName);
                        if (timeStrings[i].time.TotalMilliseconds == time && sameName) {
                            return timeStrings[i];
                        }
                    } else {
                        if (timeStrings[i].time.TotalMilliseconds == time) {
                            return timeStrings[i];
                        }
                    }
                }
            } else {
                var userStrings = timeStrings.Where(x => !string.IsNullOrEmpty(x.oName)
                    && (x.oName == names.uName || x.oName == names.dfName)).ToList();
                if (userStrings.Count > 0) {
                    return Ext.MinOf(userStrings, x => (long)x.time.TotalMilliseconds);
                }
            }
            return null;
        }

    }
}
