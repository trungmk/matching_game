# Match-3 Game Data Loading and Tile Behavior

This project is developed based on a lightweight Unity framework created by me: [unity_template_singleton](https://github.com/trungmk/unity_template_singleton). 
The framework provides modular systems such as asset management, UI, audio, scene handling, object pooling, and local data management.

## Data Loading

The game supports two types of data sources: **Remote Data** and **Local Data**.

- **Remote Data**: Loaded via WebSocket. If the remote data is `null` or unavailable, the system will automatically **fallback to Local Data**.
- **Local Data**: Loaded on demand as required by the question.

## Tile Behavior

Special handling is implemented for tiles with the type `"X"`:

- These tiles represent **Black Rocks**.
- Other tiles **cannot fall through** or pass these Black Rocks.
- Although this behavior was not explicitly required in the question, it aligns with common mechanics found in many match-3 games and has been intentionally implemented.

Thank you!