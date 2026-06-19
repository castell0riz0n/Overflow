'use server';

import {Question} from "@/lib/types";

export async function getQuestions (tag?:string): Promise<Question[]>{
    const gatewayUrl = process.env.GATEWAY_HTTP
        || process.env.services__gateway__http__0
        || 'http://localhost:8001';

    let url = `${gatewayUrl}/questions`;
    if (tag) {
        url += `?tag=${tag}`;
    }

    const res = await fetch(url);
    if (!res.ok) {
        throw new Error(`Failed to fetch questions. Status: ${res.status}`);
    }

    return res.json();
};

export async function getQuestionById (id:string): Promise<Question>{
    const gatewayUrl = process.env.GATEWAY_HTTP
        || process.env.services__gateway__http__0
        || 'http://localhost:8001';

    const url = `${gatewayUrl}/questions/${id}`;
    
    const res = await fetch(url);
    if (!res.ok) {
        throw new Error(`Failed to fetch questions. Status: ${res.status}`);
    }

    return res.json();
};