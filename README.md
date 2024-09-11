# File Searcher

Ferramenta de linha de comando feita em C# para exercitar conhecimentos com a linguagem. A instalação é simples

* Compile o programa com `dotnet run file-searcher`
* Adicione `.../file-searcher/bin/fs.exe` ao path do sistema
* digite `fs` e passe as bandeiras de buscas desejadas

Por padrão o programa busca por arquivos e pastas no diretório atual e em seus subdiretórios, se nenhum nome for passado o default a ser pesquisado é `.git`

| Bandeira | Exemplo de valor           | Função                                                     |
|----------|----------------------------|------------------------------------------------------------|
| -d       | C:/Users/Cleiton           | faz a busca em um diretório que não seja o atual           |
| -f       | .txt                       | trecho do nome de arquivo a ser pesquisado                 |
| --block  | node_modules;.vscode;.next | oculta da busca os resultados que tenham os nomes passados |

| Bandeira               | Função                                               |
|------------------------|------------------------------------------------------|
| --not-case-distinction | a busca não distingue letras maiúsculas e minúsculas |
| --block-hidden         | a busca ira ignora arquivos e diretórios ocultos     |
| --find-folders         | buscara apenas pastas                                |
| --find-files           | busca apenas arquivos                                |
| --local-search         | não busca em subdiretórios                           |




