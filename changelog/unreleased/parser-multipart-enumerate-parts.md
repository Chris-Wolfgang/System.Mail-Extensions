type: internal

`EmlParser` walks the parts of a multipart body one at a time instead of splitting the whole body into an array first, so only one part copy is live at a time.
