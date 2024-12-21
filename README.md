# Clicker
Este repositório contém a implementação de um jogo simples, desenvolvido como parte de um desafio técnico para um processo seletivo. O objetivo do desafio é criar uma aplicação que permita interação em tempo real entre múltiplos jogadores conectados via navegador, utilizando tecnologias modernas no frontend e backend.  

## Contexto do Desafio  

O desafio técnico consiste em desenvolver um jogo que atenda aos seguintes requisitos:  
- Gerenciamento de múltiplos clientes conectados simultaneamente.  
- Comunicação em tempo real entre os jogadores.  
- Controle de turnos, tempo e eliminação de participantes de acordo com as regras estabelecidas.  

### Regras do Jogo  
1. Cada jogador deve pressionar um botão o mais rápido possível para evitar acumular tempo.  
2. O backend registra o tempo de cada jogada e repassa a vez para o próximo jogador.  
3. Jogadores que acumularem mais de 30 segundos de tempo total serão eliminados.  
4. O jogo termina quando restar apenas um jogador.  

## Tecnologias Utilizadas  

Para atender aos requisitos do desafio, as seguintes tecnologias foram empregadas:  

### **Backend**  
- **.NET 8**: Utilizado para implementar a lógica do jogo e gerenciar as conexões em tempo real.  
- **SignalR**: Ferramenta para comunicação em tempo real, permitindo interação instantânea entre o backend e os clientes.  

### **Frontend**  
- **Angular**: Framework usado para desenvolver a interface gráfica interativa e responsiva.  
- **SignalR**: Integrado ao frontend para gerenciar as mensagens em tempo real.  
- **Tailwind CSS**: Biblioteca de estilos utilizada para criar um design moderno e funcional.
  
- O código do frontend está disponível neste repositório:  
[Clicker Client - Frontend](https://github.com/brantesdavi/clicker-client)  
 

---

O desenvolvimento foi concluído dentro do prazo de 3 dias, conforme solicitado. Caso precise de mais detalhes sobre a implementação ou queira visualizar o jogo em funcionamento, sinta-se à vontade para explorar este repositório!
