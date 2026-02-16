
import * as SecureStore from "expo-secure-store";
import { Platform } from "react-native";
import Constants from "expo-constants";

export const BACKEND_URL = Constants.expoConfig?.extra?.backendUrl || "http://localhost:5001";
export const BEARER_TOKEN_KEY = "minerva-foods_bearer_token";

// Get bearer token from storage
export async function getBearerToken(): Promise<string | null> {
  if (Platform.OS === "web") {
    return localStorage.getItem(BEARER_TOKEN_KEY);
  } else {
    return await SecureStore.getItemAsync(BEARER_TOKEN_KEY);
  }
}

// Set bearer token in storage
export async function setBearerToken(token: string): Promise<void> {
  if (Platform.OS === "web") {
    localStorage.setItem(BEARER_TOKEN_KEY, token);
  } else {
    await SecureStore.setItemAsync(BEARER_TOKEN_KEY, token);
  }
}

// Clear bearer token from storage
export async function clearBearerToken(): Promise<void> {
  if (Platform.OS === "web") {
    localStorage.removeItem(BEARER_TOKEN_KEY);
  } else {
    await SecureStore.deleteItemAsync(BEARER_TOKEN_KEY);
  }
}

// Generic authenticated GET request
export async function authenticatedGet<T>(url: string): Promise<T> {
  const token = await getBearerToken();
  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };
  
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  console.log(`API GET: ${BACKEND_URL}${url}`);
  const response = await fetch(`${BACKEND_URL}${url}`, {
    method: "GET",
    headers,
  });

  if (!response.ok) {
    console.error(`API GET Error: ${response.status} ${response.statusText}`);
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  const data = await response.json();
  console.log(`API GET Response:`, data);
  return data;
}

// Generic authenticated POST request
export async function authenticatedPost<T>(url: string, body: any): Promise<T> {
  const token = await getBearerToken();
  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };
  
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  console.log(`API POST: ${BACKEND_URL}${url}`, body);
  const response = await fetch(`${BACKEND_URL}${url}`, {
    method: "POST",
    headers,
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    console.error(`API POST Error: ${response.status} ${response.statusText}`);
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  const data = await response.json();
  console.log(`API POST Response:`, data);
  return data;
}

// Generic authenticated PUT request
export async function authenticatedPut<T>(url: string, body: any): Promise<T> {
  const token = await getBearerToken();
  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };
  
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  console.log(`API PUT: ${BACKEND_URL}${url}`, body);
  const response = await fetch(`${BACKEND_URL}${url}`, {
    method: "PUT",
    headers,
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    console.error(`API PUT Error: ${response.status} ${response.statusText}`);
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  const data = await response.json();
  console.log(`API PUT Response:`, data);
  return data;
}

// Generic authenticated DELETE request
export async function authenticatedDelete<T>(url: string): Promise<T> {
  const token = await getBearerToken();
  const headers: HeadersInit = {};
  
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  console.log(`API DELETE: ${BACKEND_URL}${url}`);
  const response = await fetch(`${BACKEND_URL}${url}`, {
    method: "DELETE",
    headers,
  });

  if (!response.ok) {
    console.error(`API DELETE Error: ${response.status} ${response.statusText}`);
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  const data = await response.json();
  console.log(`API DELETE Response:`, data);
  return data;
}
