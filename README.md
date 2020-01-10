**Quarry Stop Checker** utilizes the codelock from QuarryLocks. When a quarry is turned off it checks nearby players to ensure
that someone that is authorized is near by and if not turns the quarry back on.

The plugin can also be configured to announce in chat that someone tried to turn off a quarry but was thwarted and/or to slap the player(s) involved.

## Configuration

```json
{
  "EnableChatOnCaught": false,
  "EnableSlapOnCaught": false,
  "EnableServerLog": true
}
```
