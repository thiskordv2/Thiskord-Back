Ceci est le BackEnd de notre projet en C#

# Création d'un container pour faire tourner le backend :

## En mode développement avec mise à jour sur sauvegarde :

### A savoir :
Pour démarrer les machines :
``` 
docker compose up
```
Pour désactiver les logs et messages au démarrage (Pour les premiers démarrages, il est préférable de les laisser pour voir les erreurs éventuelles) :
```
docker compose up -d
```
Au démarrage des containers, Docker va créer une base de données avec un jeu de données par défaut.
Comme utilisateur par défaut, on a EMRE avec le mot de passe EMRE. \
Docker met à jour le container à chaque fois que l'on sauvegarde des fichiers de notre projet, il watch les changements de code C#

Pour arrêter les machines :
```
docker compose down
```

Pour arrêter et supprimez les volumes ( dont la base de données ) :
```

docker compose down -v
```

## En mode production :

Dans un premier temps, on build notre image :
```
docker build -t backend .
```

Dès que notre image est build, on peut la démarrer :
```
docker run -p 8080:8080 backend
```

A noter que notre configuration actuelle permet seulement de faire tourner le container sur le port 8080
