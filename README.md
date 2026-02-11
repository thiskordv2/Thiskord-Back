Ceci est le BackEnd de notre projet en C#

# Création d'un container pour faire tourner le backend :

## En mode développement avec mise à jour sur sauvegarde :

### A savoir pour n'importe quelle lancement des containers :
Docker met à jour le container à chaque fois que l'on sauvegarde des fichiers de notre projet. \
Si cela ne fonctionne pas, faire Ctrl + R sur terminal qui "watch" le container.

Pour arrêter les machines :
```
docker compose -f 'nom-du-fichier-docker-compose.yml' down
```

Pour arrêter et supprimez les volumes ( dont la base de données ) :
```

docker compose -f 'nom-du-fichier-docker-compose.yml' down -v
```

### Pour le premier lancement :
```
docker compose -f docker-compose.devinit.yml up
```
Dans ce compose, on initialise la base de données une première fois en créant la nôtre grâce au service database.configurator et au fichier Thiskord_db.sql \

Le fichier crée automatiquement un utilisateur EMRE avec le mot de passe EMRE \

⚠️Attention, ne jamais lancé cette commande deux fois de suite, car elle va essayer de recrée la base existante et faire planter le container

### Pour les lancements suivants :
``` 
docker compose -f docker-compose.dev.yml up
```

### Pour les lancements sur ASIO :
Comme on a pas besoin de crée la base de données, on doit seulement faire :
```
docker compose -f docker-compose.devasio.yml up
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
