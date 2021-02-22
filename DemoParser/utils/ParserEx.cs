using System;

namespace DemoCleaner3.DemoParser.utils {
    public class ParserEx : Exception {
        public ParserEx() : base() { }
        public ParserEx(string message) : base(message) { }
    }

    public class ErrorUnableToParseDeltaEntityState : ParserEx { public ErrorUnableToParseDeltaEntityState() : base("Unable to parse delta-entity state") { } }
    public class ErrorBadCommandInParseGameState : ParserEx { public ErrorBadCommandInParseGameState() : base("Bad command in parseGameState") { } }
    public class ErrorDeltaFrameTooOld : ParserEx { public ErrorDeltaFrameTooOld() : base("Delta frame too old.") { } }
    public class ErrorDeltaParseEntitiesNumTooOld : ParserEx { public ErrorDeltaParseEntitiesNumTooOld() : base("Delta parseEntitiesNum too old") { } }
    public class ErrorParsePacketEntitiesEndOfMessage : ParserEx { public ErrorParsePacketEntitiesEndOfMessage() : base("CL_ParsePacketEntities: end of message") { } }
    public class ErrorBaselineNumberOutOfRange : ParserEx { public ErrorBaselineNumberOutOfRange() : base("Baseline number out of range") { } }
    public class ErrorParseSnapshotInvalidsize : ParserEx { public ErrorParseSnapshotInvalidsize() : base("CL_ParseSnapshot: Invalid size for areamask") { } }
    public class ErrorDeltaFromInvalidFrame : ParserEx { public ErrorDeltaFromInvalidFrame() : base("Delta from invalid frame (not supposed to happen!)") { } }
    public class ErrorBadChecksum : ParserEx { public ErrorBadChecksum() : base("Bad checksum at decoding demo time") { } }
    public class ErrorWrongLength : ParserEx { public ErrorWrongLength() : base("Demo file is corrupted, wrong message length") { } }
    public class ErrorCantOpenFile : ParserEx { public ErrorCantOpenFile() : base("Can't open demofile") { } }
    public class ErrorInvalidFieldCount : ParserEx { public ErrorInvalidFieldCount() : base("invalid entityState field count") { } }
}
