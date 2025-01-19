
import axios from "axios";

const apiClient = axios.create({
  baseURL: "http://localhost:5209/",
});

// Interceptor לתפיסת שגיאות בתשובה
apiClient.interceptors.response.use(
  (response) => {
    // החזרת התשובה כרגיל אם הכל תקין
    return response;
  },
  (error) => {
    // רישום השגיאה בלוג
    console.error("API Error:", error.response || error.message);
    // השלכת השגיאה מחדש כדי שנוכל לטפל בה גם בפונקציות
    return Promise.reject(error);
  }
);

export default {
  // שליפת כל המשימות
  getTasks: async () => {
    try {
      const result = await apiClient.get("tasks"); // שימוש ב-apiClient במקום apiUrl
      return result.data; // הנתונים מוחזרים מה-API
    } catch (error) {
      console.error("Error fetching tasks:", error);
      throw error;
    }
  },

  // הוספת משימה חדשה
  addTask: async (name) => {
    try {
      const result = await apiClient.post("tasks", { name, isComplete: false });
      return result.data;
    } catch (error) {
      console.error("Error adding task:", error);
      throw error;
    }
  },

  // עדכון סטטוס של משימה
  setCompleted: async (id, isComplete) => {
    try {
      const result = await apiClient.put(`tasks/${id}`, { isComplete });
      return result.data;
    } catch (error) {
      console.error("Error updating task:", error);
      throw error;
    }
  },

  // מחיקת משימה
  deleteTask: async (id) => {
    try {
      await apiClient.delete(`tasks/${id}`);
      console.log(`Task ${id} deleted successfully.`);
    } catch (error) {
      console.error("Error deleting task:", error);
      throw error;
    }
  },
};


