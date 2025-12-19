# CoreChess

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/license-MIT-blue)

**CoreChess** é uma aplicação de Xadrez robusta e modular desenvolvida em C# e WPF. Este projeto foca-se na implementação de *Design Patterns*, arquitetura limpa (MVVM) e integração com motores de xadrez profissionais (Stockfish) para uma experiência PvE desafiante.



## 🚀 Funcionalidades Principais

* **Modos de Jogo:**
    * **PvP (Player vs Player):** Jogo local para dois jogadores.
    * **PvE (Player vs Engine):** Integração com **Stockfish 16** para jogar contra o computador.
    * **Escolha de Cor:** Opção para jogar de Brancas ou Pretas contra o Bot, com rotação automática do tabuleiro.
* **Lógica Completa de Xadrez:**
    * Movimentação legal de todas as peças.
    * Regras especiais: *Castling* (Roque), *En Passant* e Promoção de Peões.
    * Deteção de Xeque e Xeque-Mate.
* **Sistema de Utilizadores:**
    * Registo e Autenticação (Login) seguros.
    * Persistência de dados em JSON.
    * Histórico de partidas por utilizador.
* **Interface (UI/UX):**
    * Design moderno em WPF (Dark Mode).
    * Feedback visual de peças selecionadas e últimos movimentos.

## 🛠️ Tech Stack & Arquitetura

O projeto foi construído seguindo as melhores práticas de Engenharia de Software:

* **Linguagem:** C# (.NET 8.0)
* **Frontend:** WPF (Windows Presentation Foundation) com padrão **MVVM** (Model-View-ViewModel) para separação de responsabilidades.
* **Testes:** **xUnit** cobrindo a lógica de jogo ("Game Engine") e serviços de autenticação.
* **CI/CD:** Pipeline configurado com **GitHub Actions** para *build* e testes automáticos a cada *push*.
* **Design Patterns:**
    * **Factory Method:** Para a criação dinâmica de peças (`PieceFactory`).
    * **Singleton/Services:** Para gestão de estado e ficheiros.
    * **Observer/Binding:** Através do `INotifyPropertyChanged` do WPF.

## ⚙️ Instalação e Execução

### Pré-requisitos
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Visual Studio 2022 (ou VS Code)

### Passo a Passo
1.  **Clonar o repositório:**
    ```bash
    git clone [https://github.com/SEU_USERNAME/CoreChess.git](https://github.com/SEU_USERNAME/CoreChess.git)
    ```
2.  **Configurar o Stockfish:**
    * O jogo requer o executável do Stockfish na pasta `Engine`.
    * Certifica-te que o ficheiro `stockfish.exe` está localizado em `ChessApp.WPF/bin/Debug/net8.0-windows/Engine/` após a compilação.
3.  **Compilar e Correr:**
    * Abre a solução `ChessApp.sln`.
    * Define o projeto `ChessApp.WPF` como *Startup Project*.
    * Executa (F5).

## 🧪 Testes

O projeto possui uma suite de testes automatizada para garantir a integridade da lógica.

Para correr os testes via terminal:
```bash
dotnet test
