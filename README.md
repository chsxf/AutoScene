# ❓ About AutoScene

AutoScene allows you to work on your scene setup in the editor while loading a specific scene when entering Play mode.

## Conventions

This project uses [gitmoji](https://gitmoji.dev) for its commit messages.

# 🖥 Requirements

AutoScene requires Unity 2018.4 or later version.

# 🛠 How to Use AutoScene

Once installed, AutoScene appears as a new entry in your Unity Preferences, as displayed below.

![image](https://user-images.githubusercontent.com/3322862/123942946-e8926a80-d99b-11eb-95f6-289d00577570.png)

AutoSceen presents only two parameters: **Enable AutoScene** and **Scene to load on Play**. For the later, three modes are available:

- By selecting **None** (the default), no specific scene will be loaded and the normal behaviour of Unity when entering Play mode is maintained.
- By selection **Auto**, the first scene in the project's Build Settings will be used. If no such scene exists (no scene in the project's Build Settings), AutoScene will fall back to **None**.
- Any further entry refers to a specific scene in the project. By selecting a scene, it will be loaded when entering Play mode.
