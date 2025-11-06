// ========================================
// קובץ: service.js
// מיקום: TodoListReact/src/service.js
// תיאור: שירות שמבצע את כל הקריאות ל-API
// ========================================

import axios from 'axios';

const apiUrl = "http://localhost:5028"

// הגדרת כתובת ברירת מחדל (Config Defaults)
axios.defaults.baseURL = apiUrl;

// Interceptor לטיפול בשגיאות
axios.interceptors.response.use(
  response => response,
  error => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

const api = {
  // שליפת כל המשימות
  getTasks: async () => {
    const result = await axios.get('/items');
    return result.data;
  },

  // הוספת משימה חדשה
  addTask: async (name) => {
    console.log('addTask', name);
    const result = await axios.post('/items', {
      name: name,
      isComplete: false
    });
    return result.data;
  },

  // עדכון סטטוס משימה (הושלמה/לא הושלמה)
  setCompleted: async (todo, isComplete) => {
    console.log('setCompleted', { todo, isComplete });
    
    const result = await axios.put(`/items/${todo.id}`, {
      name: todo.name,        // שולחים את השם הקיים
      isComplete: isComplete  // מעדכנים רק את הסטטוס
    });
    return result.data;
  },

  // מחיקת משימה
  deleteTask: async (id) => {
    console.log('deleteTask', id);
    const result = await axios.delete(`/items/${id}`);
    return result.data;
  }
};

export default api;