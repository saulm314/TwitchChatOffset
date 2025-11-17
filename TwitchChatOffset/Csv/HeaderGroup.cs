using TwitchChatOffset.Options;
using System;
using System.Collections.Generic;

namespace TwitchChatOffset.Csv;

public readonly record struct HeaderGroup(Range HeaderRange, Dictionary<string, FieldData> DataMap);