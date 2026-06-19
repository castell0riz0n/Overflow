import {getQuestions} from "@/lib/actions/question-actions";
import QuestionCard from "@/app/questions/QuestionCard";
import QuestionsHeader from "@/app/questions/QuestionsHeader";

export default async function QuestionsPage( {searchParams}: { searchParams: Promise<{tag?:string}> }) {
    const params = await searchParams;
    
    const questions = await getQuestions(params?.tag);
    
    return (
        <>
            <QuestionsHeader total={questions.length} tag={params?.tag} />
            {questions.map(q => (
                <div key={q.id} className="flex w-full py-4 not-last:border-b">
                    <QuestionCard key={q.id} question={q}/>
                </div>                
            ))}

        </>
    );
}