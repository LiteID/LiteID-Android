# LiteID-Android
Android app for LiteID

Requirements:
- Must-have:
  - Set up new identity
    - Create Etheruem account
    - Show instructions to add Gas to account
    - Create contracts from that account
  - Have visual list of user's documents associated with their identity, such as:
    - Name, Address, SSN
    - Documents, like Word or PDF
    - Images or any other file
    - Any text or number
  - Add document to app by sharing
  - Add document to app by entering text
  - When adding document:
    - Make internal copy of file
      - This is in case the user inadvertently changes the file
    - Hash file and add hash to blockchain
    - Add file to list of user's documents
  - Share document to other user which can be verified by their LiteID app
    - Current concept is to send self-contained file by email
- Nice-to-have:
  - Add document to app by selecting file
  - Export self-contained file of all documents and metadata
- Optional:
  - Dropbox/OneDrive sync
  - Coinbase integration to automagically obtain Ethereum Gas