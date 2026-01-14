# 🎮 GameNetTwo - 3D OpenGL Rendering with Silk.NET 🚀

Welcome to **GameNetTwo**! This is a high-performance 3D graphics demonstration built using **C#**, **.NET 10**, and the powerful **Silk.NET** library to interface with **OpenGL**.

---

## 🌟 Overview

This project showcases a fundamental 3D rendering pipeline, featuring a vibrant, rotating multi-colored cube in a 3D space. It serves as a solid foundation for building more complex game engines or graphical applications in the .NET ecosystem.

![3D Graphics](https://img.shields.io/badge/Graphics-OpenGL-blue?style=for-the-badge&logo=opengl)
![Platform](https://img.shields.io/badge/Platform-.NET%2010-purple?style=for-the-badge&logo=dotnet)

---

## ✨ Features

- 🧊 **3D Geometry**: Rendering a full 3D cube with individual vertex colors.
- 🔄 **Real-time Rotation**: Smooth X and Y axis rotation using mathematical transformations.
- 🎨 **Custom Shaders**: GLSL vertex and fragment shaders for high-performance rendering.
- ⌨️ **Input Handling**: Integrated keyboard support via Silk.NET Input.
- 📐 **Perspective Projection**: Realistic 3D depth using field-of-view matrices.
- 📱 **Responsive Design**: Window resizing support with viewport adjustment.

---

## 🛠️ Tech Stack

- **Language:** [C# 14.0](https://learn.microsoft.com/en-us/dotnet/csharp/) 🖥️
- **Framework:** [.NET 10.0](https://dotnet.microsoft.com/download) 💎
- **Graphics Library:** [Silk.NET](https://github.com/dotnet/Silk.NET) (OpenGL bindings) 🔌
- **Math:** `System.Numerics` for vector and matrix operations 🧮

---

## 🚀 Getting Started

### Prerequisites

Make sure you have the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) installed on your machine.

### Installation & Running

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-repo/GameNetTwo.git
   cd GameNetTwo
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   dotnet run --project GameNetTwo
   ```

---

## 🎮 Controls

| Key | Action |
|-----|--------|
| `ESC` | Close the application 🚪 |

---

## 📁 Project Structure

- `GameNetTwo/Program.cs`: The main entry point and rendering loop logic.
- `GameNetTwo/Shader.cs`: A helper class for managing OpenGL shader programs.
- `GameNetTwo/Shaders/`: Contains the `.vert` (Vertex) and `.frag` (Fragment) GLSL files.

---

## 📜 License

This project is open-source. Feel free to use and modify it for your own learning or projects! 🆓

---

Happy coding! 👩‍💻👨‍💻
