import { Moment } from "moment";
import * as React from "react";

export const LogEntry: React.SFC<{timestamp: Moment}> = ({timestamp, children}) =>
    <li>{timestamp.format("HH:mm")}: {children}</li>
