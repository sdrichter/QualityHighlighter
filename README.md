# QualityHighlighter
QualityHighlighter plugin for KeePass 2.x.
Copyright (C) 2015 by Scott Richter (scott.d.richter at gmail.com)

QualityHighlighter is a simple plugin for KeePass that highlights entries based on the password quality. Highlighting is done in 5 categories (based on the KeePass website at http://keepass.info/help/kb/pw_quality_est.html) as follows:

1. Very Weak - <= 64 bits - Red
2. Weak - 64-80 bits - Orange
3. Moderate - 81-112 bits - Yellow
4. Strong - 113-128 bits - Yellow-Green
5. Very Strong - >128 bits - Green

The plugin features a button on the KeePass toolbar to toggle the highlights on and off.

Future improvement plan is to have an options dialog to customize quality levels, including the bit cutoffs and highlight colors.

This plugin pairs very well with the QualityColumn plugin available on the KeePass website at http://keepass.info/plugins.html#qcol.

QualityHighlighter is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

Version History:
v1.1: Corrected the toggle option so it retains the alternating background colors that are default in KeePass when toggled off rather than setting them all to transparent.

v1.0: Initial release with basic highlighting functionality and toggle option.