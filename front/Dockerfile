# Stage 1: Build the Angular app
FROM node:20 as build

# Устанавливаем рабочую директорию внутри контейнера
WORKDIR /app

# Копируем package.json и package-lock.json для установки зависимостей
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем остальные файлы проекта
COPY . .

# Собираем Angular приложение для продакшена
RUN npm run build --prod

# Stage 2: Serve the app with Nginx
FROM nginx:alpine

# Копируем собранное приложение в папку, которую обслуживает Nginx
COPY --from=build /app/dist/online-chess /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

# Открываем порт 80 для доступа к приложению
EXPOSE 80 4200

# Запускаем Nginx
CMD ["nginx", "-g", "daemon off;"]
