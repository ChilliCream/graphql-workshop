# Middleware flow

```mermaid
sequenceDiagram
  autonumber
  UsePaging->>+UseProjection: next(context)
  UseProjection->>+UseFiltering: next(context)
  UseFiltering->>+UseSorting: next(context)
  UseSorting->>+Resolver: next(context)

  Resolver-->>-UseSorting: apply sorting
  UseSorting-->>-UseFiltering: apply filtering
  UseFiltering-->>-UseProjection: apply projections
  UseProjection-->>-UsePaging: apply paging
```

---

[Mermaid Live Editor](https://mermaid.live/edit#pako:eNp9kstOg0AUhl-FnJVG2gilXGbRlXFnYmzcGDYjnFIUZnAupkh4d4drSVplM_wz33cyOWcaSHiKQEDil0aW4ENOM0HLmFnmo1pxpst3FEN-lfhMs5xlq93urguCf2Cics6IxfCkbhLOlFlvz_hMjMpjXigUpsRfxgyMwp4L9Q8-HnfwC0pefKO4QAd4Ol4ZdrUsTKuqqC05xIvCI72498Afpo0rNx-dZXsGqZp35LUWTV7f49npE9hQoihpnpphNZ0cgzpiiTEQ85tS8RlDzFrDdVPb1ywBooRGGwTX2RHIgRbSJF2lVE1jnhBMc8XF0_AU-hdhQ0XZG-flLJoMpIETEG-7djw3DCIv9O_DTRR6NtRAgnDtBIGzidzQdV1_67c2_PQVnPYXPefPyg) (export SVG)
